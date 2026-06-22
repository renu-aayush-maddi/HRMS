using ClosedXML.Excel;
using HRMS.API.Common.Constants;
using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.Bonus;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Services;

public class BonusService : IBonusService
{
    private readonly IBonusRepository repository;
    private readonly IUserContextService userContextService;
    private readonly IAuditLogService auditLogService;
    private readonly INotificationService notificationService;
    private readonly ILogger<BonusService> logger;

    public BonusService(
        IBonusRepository repository,
        IUserContextService userContextService,
        IAuditLogService auditLogService,
        INotificationService notificationService,
        ILogger<BonusService> logger)
    {
        this.repository = repository;
        this.userContextService = userContextService;
        this.auditLogService = auditLogService;
        this.notificationService = notificationService;
        this.logger = logger;
    }

    public async Task<BonusResponseDto> CreateBonusAsync(
        CreateBonusDto dto,
        CancellationToken cancellationToken = default)
    {
        var employee = await repository.GetEmployeeAsync(dto.EmployeeId, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException("Employee not found.");
        }

        var exists = await repository.BonusExistsAsync(dto.EmployeeId, dto.Reason, dto.BonusMonth, dto.BonusYear, cancellationToken);
        if (exists)
        {
            throw new BusinessException("Bonus already exists for this employee, month and year.");
        }

        var bonus = new Bonuse
        {
            Id = Guid.NewGuid(),
            EmployeeId = dto.EmployeeId,
            Amount = dto.Amount,
            Reason = dto.Reason,
            BonusMonth = dto.BonusMonth,
            BonusYear = dto.BonusYear,
            Status = BonusStatuses.Pending,
            CreatedAt = DateTime.Now,
            IsProcessed = false
        };

        await repository.AddBonusAsync(bonus, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Create",
            nameof(Bonuse),
            bonus.Id,
            $"Bonus created for employee {employee.Id}",
            cancellationToken);

        logger.LogInformation("Bonus {BonusId} created for employee {EmployeeId}", bonus.Id, employee.Id);

        return new BonusResponseDto
        {
            Id = bonus.Id,
            EmployeeId = employee.Id,
            EmployeeName = $"{employee.FirstName} {employee.LastName}",
            Amount = bonus.Amount,
            Reason = bonus.Reason,
            BonusMonth = bonus.BonusMonth,
            BonusYear = bonus.BonusYear,
            Status = bonus.Status ?? string.Empty,
            CreatedAt = bonus.CreatedAt
        };
    }

    public async Task ApproveBonusAsync(
        Guid bonusId,
        CancellationToken cancellationToken = default)
    {
        var bonus = await repository.GetBonusAsync(bonusId, cancellationToken);
        if (bonus == null)
        {
            throw new NotFoundException("Bonus not found.");
        }

        if (bonus.IsProcessed == true)
        {
            throw new BusinessException("Bonus already processed in payroll.");
        }

        if (bonus.Status != BonusStatuses.Pending)
        {
            throw new BusinessException("Bonus already processed.");
        }

        bonus.Status = BonusStatuses.Approved;

        repository.UpdateBonus(bonus);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Approve",
            nameof(Bonuse),
            bonus.Id,
            "Bonus approved.",
            cancellationToken);

        if (bonus.Employee.UserId.HasValue)
        {
            notificationService.CreateNotification(
                bonus.Employee.UserId.Value,
                "Bonus Approved",
                $"Bonus of {bonus.Amount} approved.");
        }

        logger.LogInformation("Bonus {BonusId} approved", bonus.Id);
    }

    public async Task RejectBonusAsync(
        Guid bonusId,
        CancellationToken cancellationToken = default)
    {
        var bonus = await repository.GetBonusAsync(bonusId, cancellationToken);
        if (bonus == null)
        {
            throw new NotFoundException("Bonus not found.");
        }

        if (bonus.IsProcessed == true)
        {
            throw new BusinessException("Bonus already processed in payroll.");
        }

        if (bonus.Status != BonusStatuses.Pending)
        {
            throw new BusinessException("Bonus already processed.");
        }

        bonus.Status = BonusStatuses.Rejected;

        repository.UpdateBonus(bonus);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Reject",
            nameof(Bonuse),
            bonus.Id,
            "Bonus rejected.",
            cancellationToken);

        if (bonus.Employee.UserId.HasValue)
        {
            notificationService.CreateNotification(
                bonus.Employee.UserId.Value,
                "Bonus Rejected",
                $"Bonus of {bonus.Amount} rejected.");
        }

        logger.LogInformation("Bonus {BonusId} rejected", bonus.Id);
    }

    public async Task<BonusResponseDto> GetBonusAsync(
        Guid bonusId,
        CancellationToken cancellationToken = default)
    {
        var bonus = await repository.GetBonusAsync(bonusId, cancellationToken);
        if (bonus == null)
        {
            throw new NotFoundException("Bonus not found.");
        }

        return MapToResponse(bonus);
    }

    public async Task<PagedResponse<BonusResponseDto>> GetBonusesAsync(
        BonusFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var query = repository.GetBonuses();

        if (!userContextService.IsAdminOrHr())
        {
            var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
            if (employeeId == null)
            {
                throw new NotFoundException("Employee profile not found.");
            }

            query = query.Where(x => x.EmployeeId == employeeId);
        }

        query = ApplyFilters(query, filter);

        var totalRecords = await query.CountAsync(cancellationToken);
        var bonuses = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<BonusResponseDto>
        {
            Data = bonuses.Select(MapToResponse),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize)
        };
    }

    public async Task<PagedResponse<BonusResponseDto>> GetMyBonusesAsync(
        BonusFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
        if (employeeId == null)
        {
            throw new NotFoundException("Employee profile not found.");
        }

        filter.EmployeeId = employeeId;
        return await GetBonusesAsync(filter, cancellationToken);
    }

    public async Task<byte[]> ExportBonusesAsync(
        BonusFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilters(repository.GetBonuses(), filter);
        var bonuses = await query.ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Bonuses");

        worksheet.Cell(1, 1).Value = "Employee";
        worksheet.Cell(1, 2).Value = "Amount";
        worksheet.Cell(1, 3).Value = "Reason";
        worksheet.Cell(1, 4).Value = "Month";
        worksheet.Cell(1, 5).Value = "Year";
        worksheet.Cell(1, 6).Value = "Status";

        var row = 2;
        foreach (var bonus in bonuses)
        {
            worksheet.Cell(row, 1).Value = $"{bonus.Employee.FirstName} {bonus.Employee.LastName}";
            worksheet.Cell(row, 2).Value = bonus.Amount;
            worksheet.Cell(row, 3).Value = bonus.Reason;
            worksheet.Cell(row, 4).Value = bonus.BonusMonth;
            worksheet.Cell(row, 5).Value = bonus.BonusYear;
            worksheet.Cell(row, 6).Value = bonus.Status;
            row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        await auditLogService.LogAsync(
            "Export",
            nameof(Bonuse),
            Guid.Empty,
            $"{bonuses.Count} bonuses exported.",
            cancellationToken);

        return stream.ToArray();
    }

    private static IQueryable<Bonuse> ApplyFilters(
        IQueryable<Bonuse> query,
        BonusFilterDto filter)
    {
        if (filter.EmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == filter.EmployeeId);
        }

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            query = query.Where(x => x.Status == filter.Status);
        }

        if (filter.BonusMonth.HasValue)
        {
            query = query.Where(x => x.BonusMonth == filter.BonusMonth);
        }

        if (filter.BonusYear.HasValue)
        {
            query = query.Where(x => x.BonusYear == filter.BonusYear);
        }

        if (filter.MinAmount.HasValue)
        {
            query = query.Where(x => x.Amount >= filter.MinAmount);
        }

        if (filter.MaxAmount.HasValue)
        {
            query = query.Where(x => x.Amount <= filter.MaxAmount);
        }

        return query.OrderByDescending(x => x.CreatedAt);
    }

    private static BonusResponseDto MapToResponse(Bonuse bonus)
    {
        return new BonusResponseDto
        {
            Id = bonus.Id,
            EmployeeId = bonus.EmployeeId,
            EmployeeName = $"{bonus.Employee.FirstName} {bonus.Employee.LastName}",
            Amount = bonus.Amount,
            Reason = bonus.Reason,
            BonusMonth = bonus.BonusMonth,
            BonusYear = bonus.BonusYear,
            Status = bonus.Status ?? string.Empty,
            CreatedAt = bonus.CreatedAt
        };
    }
}
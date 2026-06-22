using ClosedXML.Excel;
using HRMS.API.Common.Constants;
using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.Deduction;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Services;

public class DeductionService : IDeductionService
{
    private readonly IDeductionRepository repository;
    private readonly IUserContextService userContextService;
    private readonly IAuditLogService auditLogService;
    private readonly INotificationService notificationService;
    private readonly ILogger<DeductionService> logger;

    public DeductionService(
        IDeductionRepository repository,
        IUserContextService userContextService,
        IAuditLogService auditLogService,
        INotificationService notificationService,
        ILogger<DeductionService> logger)
    {
        this.repository = repository;
        this.userContextService = userContextService;
        this.auditLogService = auditLogService;
        this.notificationService = notificationService;
        this.logger = logger;
    }

    public async Task<DeductionResponseDto> CreateDeductionAsync(
        CreateDeductionDto dto,
        CancellationToken cancellationToken = default)
    {
        var employee = await repository.GetEmployeeAsync(dto.EmployeeId, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException("Employee not found.");
        }

        var exists = await repository.DeductionExistsAsync(
            dto.EmployeeId,
            dto.Reason,
            dto.DeductionMonth,
            dto.DeductionYear,
            cancellationToken);

        if (exists)
        {
            throw new BusinessException("Deduction already exists for this employee, month and year.");
        }

        var deduction = new Deduction
        {
            Id = Guid.NewGuid(),
            EmployeeId = dto.EmployeeId,
            Amount = dto.Amount,
            Reason = dto.Reason,
            DeductionMonth = dto.DeductionMonth,
            DeductionYear = dto.DeductionYear,
            Status = BonusStatuses.Pending,
            CreatedAt = DateTime.Now,
            IsProcessed = false
        };

        await repository.AddDeductionAsync(deduction, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Create",
            nameof(Deduction),
            deduction.Id,
            $"Deduction created for employee {employee.Id}",
            cancellationToken);

        logger.LogInformation("Deduction {DeductionId} created for employee {EmployeeId}", deduction.Id, employee.Id);

        return new DeductionResponseDto
        {
            Id = deduction.Id,
            EmployeeName = $"{employee.FirstName} {employee.LastName}",
            Amount = deduction.Amount,
            Reason = deduction.Reason,
            DeductionMonth = deduction.DeductionMonth,
            DeductionYear = deduction.DeductionYear,
            Status = deduction.Status ?? string.Empty
        };
    }

    public async Task ApproveDeductionAsync(
        Guid deductionId,
        CancellationToken cancellationToken = default)
    {
        var deduction = await repository.GetDeductionAsync(deductionId, cancellationToken);
        if (deduction == null)
        {
            throw new NotFoundException("Deduction not found.");
        }

        if (deduction.IsProcessed == true)
        {
            throw new BusinessException("Deduction already processed in payroll.");
        }

        if (deduction.Status != BonusStatuses.Pending)
        {
            throw new BusinessException("Deduction already processed.");
        }

        deduction.Status = BonusStatuses.Approved;

        repository.UpdateDeduction(deduction);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Approve",
            nameof(Deduction),
            deduction.Id,
            "Deduction approved.",
            cancellationToken);

        if (deduction.Employee.UserId.HasValue)
        {
            notificationService.CreateNotification(
                deduction.Employee.UserId.Value,
                "Deduction Approved",
                $"Deduction of {deduction.Amount} approved.");
        }

        logger.LogInformation("Deduction {DeductionId} approved", deduction.Id);
    }

    public async Task RejectDeductionAsync(
        Guid deductionId,
        CancellationToken cancellationToken = default)
    {
        var deduction = await repository.GetDeductionAsync(deductionId, cancellationToken);
        if (deduction == null)
        {
            throw new NotFoundException("Deduction not found.");
        }

        if (deduction.IsProcessed == true)
        {
            throw new BusinessException("Deduction already processed in payroll.");
        }

        if (deduction.Status != BonusStatuses.Pending)
        {
            throw new BusinessException("Deduction already processed.");
        }

        deduction.Status = BonusStatuses.Rejected;

        repository.UpdateDeduction(deduction);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Reject",
            nameof(Deduction),
            deduction.Id,
            "Deduction rejected.",
            cancellationToken);

        if (deduction.Employee.UserId.HasValue)
        {
            notificationService.CreateNotification(
                deduction.Employee.UserId.Value,
                "Deduction Rejected",
                $"Deduction of {deduction.Amount} rejected.");
        }

        logger.LogInformation("Deduction {DeductionId} rejected", deduction.Id);
    }

    public async Task<DeductionResponseDto> GetDeductionAsync(
        Guid deductionId,
        CancellationToken cancellationToken = default)
    {
        var deduction = await repository.GetDeductionAsync(deductionId, cancellationToken);
        if (deduction == null)
        {
            throw new NotFoundException("Deduction not found.");
        }

        return MapToResponse(deduction);
    }

    public async Task<PagedResponse<DeductionResponseDto>> GetDeductionsAsync(
        DeductionFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var query = repository.GetDeductions();

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
        var deductions = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<DeductionResponseDto>
        {
            Data = deductions.Select(MapToResponse),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize)
        };
    }

    public async Task<PagedResponse<DeductionResponseDto>> GetMyDeductionsAsync(
        DeductionFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
        if (employeeId == null)
        {
            throw new NotFoundException("Employee profile not found.");
        }

        filter.EmployeeId = employeeId;
        return await GetDeductionsAsync(filter, cancellationToken);
    }

    public async Task<byte[]> ExportDeductionsAsync(
        DeductionFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilters(repository.GetDeductions(), filter);
        var deductions = await query.ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Deductions");

        worksheet.Cell(1, 1).Value = "Employee";
        worksheet.Cell(1, 2).Value = "Amount";
        worksheet.Cell(1, 3).Value = "Reason";
        worksheet.Cell(1, 4).Value = "Month";
        worksheet.Cell(1, 5).Value = "Year";
        worksheet.Cell(1, 6).Value = "Status";

        var row = 2;
        foreach (var deduction in deductions)
        {
            worksheet.Cell(row, 1).Value = $"{deduction.Employee.FirstName} {deduction.Employee.LastName}";
            worksheet.Cell(row, 2).Value = deduction.Amount;
            worksheet.Cell(row, 3).Value = deduction.Reason;
            worksheet.Cell(row, 4).Value = deduction.DeductionMonth;
            worksheet.Cell(row, 5).Value = deduction.DeductionYear;
            worksheet.Cell(row, 6).Value = deduction.Status;
            row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        await auditLogService.LogAsync(
            "Export",
            nameof(Deduction),
            Guid.Empty,
            $"{deductions.Count} deductions exported.",
            cancellationToken);

        return stream.ToArray();
    }

    private static IQueryable<Deduction> ApplyFilters(
        IQueryable<Deduction> query,
        DeductionFilterDto filter)
    {
        if (filter.EmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == filter.EmployeeId);
        }

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            query = query.Where(x => x.Status == filter.Status);
        }

        if (filter.DeductionMonth.HasValue)
        {
            query = query.Where(x => x.DeductionMonth == filter.DeductionMonth);
        }

        if (filter.DeductionYear.HasValue)
        {
            query = query.Where(x => x.DeductionYear == filter.DeductionYear);
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

    private static DeductionResponseDto MapToResponse(Deduction deduction)
    {
        return new DeductionResponseDto
        {
            Id = deduction.Id,
            EmployeeName = $"{deduction.Employee.FirstName} {deduction.Employee.LastName}",
            Amount = deduction.Amount,
            Reason = deduction.Reason,
            DeductionMonth = deduction.DeductionMonth,
            DeductionYear = deduction.DeductionYear,
            Status = deduction.Status ?? string.Empty
        };
    }
}
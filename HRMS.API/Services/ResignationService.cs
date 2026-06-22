using ClosedXML.Excel;
using HRMS.API.Constants;
using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.Resignation;
using HRMS.API.Models.Entities;
using HRMS.API.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Services;

public class EmployeeResignationService : IEmployeeResignationService
{
    private readonly IEmployeeResignationRepository repository;
    private readonly IEmployeeAccessResolver accessResolver;
    private readonly IUserContextService userContextService;
    private readonly IAuditLogService auditLogService;
    private readonly ILogger<EmployeeResignationService> logger;

    public EmployeeResignationService(
        IEmployeeResignationRepository repository,
        IEmployeeAccessResolver accessResolver,
        IUserContextService userContextService,
        IAuditLogService auditLogService,
        ILogger<EmployeeResignationService> logger)
    {
        this.repository = repository;
        this.accessResolver = accessResolver;
        this.userContextService = userContextService;
        this.auditLogService = auditLogService;
        this.logger = logger;
    }

    public async Task<EmployeeResignationDto> CreateAsync(CreateResignationDto dto, CancellationToken cancellationToken = default)
    {
        var employeeId =
    await userContextService.GetEmployeeIdAsync(
        cancellationToken);

        if (employeeId == null)
        {
            throw new NotFoundException(
                "Employee not found.");
        }
        var employee = await repository.GetEmployeeAsync(employeeId.Value, cancellationToken);

        if (employee is null) throw new NotFoundException("Employee not found.");
        if (employee.EmploymentStatus != EmployeeStatus.Active.ToString())
            throw new BusinessException("Only active employees can submit resignations.");

        var existingResignation =await repository.GetActiveResignationAsync(
        employeeId.Value,
        cancellationToken);
        if (existingResignation is not null) throw new BusinessException("An active resignation already exists.");

        if (dto.LastWorkingDate < DateOnly.FromDateTime(DateTime.Now).AddDays(30))
            throw new BusinessException("Minimum notice period is 30 days.");

        var resignation = new EmployeeResignation
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId.Value,
            ResignationDate = DateOnly.FromDateTime(DateTime.Now),
            LastWorkingDate = dto.LastWorkingDate,
            Reason = dto.Reason,
            Status = ResignationStatuses.Pending,
            FinalSettlementStatus = SettlementStatuses.Pending,
            CreatedAt = DateTime.Now
        };

        await repository.AddAsync(resignation, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Create", nameof(EmployeeResignation), resignation.Id, "Resignation submitted.", cancellationToken);
        logger.LogInformation("Resignation created: {ResignationId}", resignation.Id);

        return await GetByIdAsync(resignation.Id, cancellationToken);
    }

    public async Task<EmployeeResignationDto> GetByIdAsync(Guid resignationId, CancellationToken cancellationToken = default)
    {
        var resignation = await repository.GetByIdWithEmployeeAsync(resignationId, cancellationToken);
        if (resignation is null) throw new NotFoundException("Resignation not found.");

        if (!userContextService.IsAdminOrHr())
            await accessResolver.ValidateEmployeeOwnershipAsync(resignation.EmployeeId, cancellationToken);

        return MapToDto(resignation);
    }

    public async Task<PagedResult<EmployeeResignationDto>> GetAllAsync(ResignationFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = repository.GetQueryable();

        if (!userContextService.IsAdminOrHr())
        {
            var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
            query = query.Where(x => x.EmployeeId == employeeId);
        }

        if (filter.EmployeeId.HasValue) query = query.Where(x => x.EmployeeId == filter.EmployeeId);
        if (!string.IsNullOrWhiteSpace(filter.Status)) query = query.Where(x => x.Status == filter.Status);
        if (!string.IsNullOrWhiteSpace(filter.FinalSettlementStatus)) query = query.Where(x => x.FinalSettlementStatus == filter.FinalSettlementStatus);
        if (filter.FromResignationDate.HasValue) query = query.Where(x => x.ResignationDate >= filter.FromResignationDate.Value);
        if (filter.ToResignationDate.HasValue) query = query.Where(x => x.ResignationDate <= filter.ToResignationDate.Value);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var search = filter.SearchTerm.ToLower();
            query = query.Where(x => x.Employee.FirstName.ToLower().Contains(search) ||
                                     x.Employee.LastName.ToLower().Contains(search) ||
                                     x.Employee.EmployeeCode.ToLower().Contains(search));
        }

        query = filter.SortBy?.ToLower() switch
        {
            "resignationdate" => filter.SortDescending ? query.OrderByDescending(x => x.ResignationDate) : query.OrderBy(x => x.ResignationDate),
            "lastworkingdate" => filter.SortDescending ? query.OrderByDescending(x => x.LastWorkingDate) : query.OrderBy(x => x.LastWorkingDate),
            "status" => filter.SortDescending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            _ => filter.SortDescending ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt)
        };

        var totalRecords = await query.CountAsync(cancellationToken);
        var resignations = await query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync(cancellationToken);

        return new PagedResult<EmployeeResignationDto>
        {
            Data = resignations.Select(MapToDto).ToList(),
            Page = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalRecords
        };
    }

    public async Task ApproveAsync(Guid resignationId, CancellationToken cancellationToken = default)
    {
        var resignation = await repository.GetByIdAsync(resignationId, cancellationToken);
        if (resignation is null) throw new NotFoundException("Resignation not found.");
        if (resignation.Status != ResignationStatuses.Pending) throw new BusinessException("Only pending resignations can be approved.");

        var employee = await repository.GetEmployeeAsync(resignation.EmployeeId, cancellationToken);
        resignation.Status = ResignationStatuses.Approved;
        resignation.ApprovedBy = await userContextService.GetEmployeeIdAsync(cancellationToken);
        resignation.ApprovedAt = DateTime.Now;
        resignation.UpdatedAt = DateTime.Now;

        if (employee is not null)
        {
            employee.EmploymentStatus = EmployeeStatus.NoticePeriod.ToString();
            await repository.UpdateEmployeeAsync(employee, cancellationToken);
        }

        await repository.UpdateAsync(resignation, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Approve", nameof(EmployeeResignation), resignation.Id, "Resignation approved.", cancellationToken);
    }

    private static EmployeeResignationDto MapToDto(EmployeeResignation resignation) => new()
    {
        Id = resignation.Id,
        EmployeeId = resignation.EmployeeId,
        EmployeeCode = resignation.Employee.EmployeeCode,
        EmployeeName = $"{resignation.Employee.FirstName} {resignation.Employee.LastName}",
        ResignationDate = resignation.ResignationDate,
        LastWorkingDate = resignation.LastWorkingDate,
        Reason = resignation.Reason,
        Status = resignation.Status ?? string.Empty,
        HrComments = resignation.HrComments,
        FinalSettlementStatus = resignation.FinalSettlementStatus ?? string.Empty,
        ApprovedBy = resignation.ApprovedBy,
        ApprovedAt = resignation.ApprovedAt,
        RejectedBy = resignation.RejectedBy,
        RejectedAt = resignation.RejectedAt,
        WithdrawnAt = resignation.WithdrawnAt,
        CreatedAt = resignation.CreatedAt,
        UpdatedAt = resignation.UpdatedAt
    };

    public async Task RejectAsync(Guid resignationId, RejectResignationDto dto, CancellationToken cancellationToken = default)
    {
        var resignation = await repository.GetByIdAsync(resignationId, cancellationToken);

        if (resignation is null)
        {
            throw new NotFoundException("Resignation not found.");
        }

        if (resignation.Status != ResignationStatuses.Pending)
        {
            throw new BusinessException("Only pending resignations can be rejected.");
        }

        var reviewerEmployeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);

        resignation.Status = ResignationStatuses.Rejected;
        resignation.HrComments = dto.HrComments;
        resignation.RejectedBy = reviewerEmployeeId;
        resignation.RejectedAt = DateTime.Now;
        resignation.UpdatedAt = DateTime.Now;

        await repository.UpdateAsync(resignation, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Reject",
            nameof(EmployeeResignation),
            resignation.Id,
            "Resignation rejected.",
            cancellationToken);

        logger.LogInformation("Resignation rejected: {ResignationId}", resignation.Id);
    }

    public async Task WithdrawAsync(Guid resignationId, CancellationToken cancellationToken = default)
    {
        var resignation = await repository.GetByIdAsync(resignationId, cancellationToken);

        if (resignation is null)
        {
            throw new NotFoundException("Resignation not found.");
        }

        await accessResolver.ValidateEmployeeOwnershipAsync(resignation.EmployeeId, cancellationToken);

        if (resignation.Status != ResignationStatuses.Pending)
        {
            throw new BusinessException("Only pending resignations can be withdrawn.");
        }

        resignation.Status = ResignationStatuses.Withdrawn;
        resignation.WithdrawnAt = DateTime.Now;
        resignation.UpdatedAt = DateTime.Now;

        await repository.UpdateAsync(resignation, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Withdraw",
            nameof(EmployeeResignation),
            resignation.Id,
            "Resignation withdrawn.",
            cancellationToken);

        logger.LogInformation("Resignation withdrawn: {ResignationId}", resignation.Id);
    }


    public async Task UpdateSettlementStatusAsync(
    Guid resignationId,
    UpdateSettlementStatusDto dto,
    CancellationToken cancellationToken = default)
    {
        var resignation = await repository.GetByIdAsync(resignationId, cancellationToken);

        if (resignation is null)
        {
            throw new NotFoundException("Resignation not found.");
        }

        var currentStatus = resignation.FinalSettlementStatus ?? SettlementStatuses.Pending;
        var requestedStatus = dto.FinalSettlementStatus;

        var validTransition = (currentStatus == SettlementStatuses.Pending && requestedStatus == SettlementStatuses.Processing)
                              || (currentStatus == SettlementStatuses.Processing && requestedStatus == SettlementStatuses.Completed);

        if (!validTransition)
        {
            throw new BusinessException($"Invalid settlement status transition from {currentStatus} to {requestedStatus}.");
        }

        resignation.FinalSettlementStatus = requestedStatus;
        resignation.UpdatedAt = DateTime.Now;

        await repository.UpdateAsync(resignation, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Settlement Update",
            nameof(EmployeeResignation),
            resignation.Id,
            $"Settlement status changed to {requestedStatus}.",
            cancellationToken);

        logger.LogInformation("Settlement updated for resignation: {ResignationId}", resignation.Id);
    }

    public async Task<byte[]> ExportAsync(ResignationFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = repository.GetQueryable();

        if (filter.EmployeeId.HasValue)
            query = query.Where(x => x.EmployeeId == filter.EmployeeId);

        if (!string.IsNullOrWhiteSpace(filter.Status))
            query = query.Where(x => x.Status == filter.Status);

        if (!string.IsNullOrWhiteSpace(filter.FinalSettlementStatus))
            query = query.Where(x => x.FinalSettlementStatus == filter.FinalSettlementStatus);

        var resignations = await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Resignations");

        worksheet.Cell(1, 1).Value = "Employee Code";
        worksheet.Cell(1, 2).Value = "Employee Name";
        worksheet.Cell(1, 3).Value = "Resignation Date";
        worksheet.Cell(1, 4).Value = "Last Working Date";
        worksheet.Cell(1, 5).Value = "Status";
        worksheet.Cell(1, 6).Value = "Settlement Status";
        worksheet.Cell(1, 7).Value = "HR Comments";

        var row = 2;
        foreach (var resignation in resignations)
        {
            worksheet.Cell(row, 1).Value = resignation.Employee.EmployeeCode;
            worksheet.Cell(row, 2).Value = $"{resignation.Employee.FirstName} {resignation.Employee.LastName}";
            worksheet.Cell(row, 3).Value = resignation.ResignationDate.ToString();
            worksheet.Cell(row, 4).Value = resignation.LastWorkingDate?.ToString();
            worksheet.Cell(row, 5).Value = resignation.Status;
            worksheet.Cell(row, 6).Value = resignation.FinalSettlementStatus;
            worksheet.Cell(row, 7).Value = resignation.HrComments;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
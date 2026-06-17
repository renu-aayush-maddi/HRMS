using HRMS.API.Common.Constants;
using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.AttendanceRegularization;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Services;

public class AttendanceRegularizationService : IAttendanceRegularizationService
{
    private const int MaxBackdatedDays = 30;

    private readonly IAttendanceRegularizationRepository repository;
    private readonly IEmployeeAccessResolver accessResolver;
    private readonly IUserContextService userContextService;
    private readonly IAuditLogService auditLogService;
    private readonly ILogger<AttendanceRegularizationService> logger;

    public AttendanceRegularizationService(
        IAttendanceRegularizationRepository repository,
        IEmployeeAccessResolver accessResolver,
        IUserContextService userContextService,
        IAuditLogService auditLogService,
        ILogger<AttendanceRegularizationService> logger)
    {
        this.repository = repository;
        this.accessResolver = accessResolver;
        this.userContextService = userContextService;
        this.auditLogService = auditLogService;
        this.logger = logger;
    }

    public async Task<AttendanceRegularizationResponseDto> CreateRequestAsync(CreateAttendanceRegularizationDto dto, CancellationToken cancellationToken = default)
    {
        var employeeId = await accessResolver.ResolveEmployeeIdAsync(dto.EmployeeId, cancellationToken);
        await accessResolver.ValidateEmployeeOwnershipAsync(employeeId, cancellationToken);

        var employee = await repository.GetEmployeeAsync(employeeId, cancellationToken);
        if (employee == null) throw new NotFoundException("Employee not found.");

        if (dto.AttendanceDate > DateOnly.FromDateTime(DateTime.Now))
            throw new BusinessException("Future attendance dates are not allowed.");

        if (dto.AttendanceDate < DateOnly.FromDateTime(DateTime.Now.AddDays(-MaxBackdatedDays)))
            throw new BusinessException($"Attendance regularization is allowed only for the last {MaxBackdatedDays} days.");

        var attendance = await repository.GetAttendanceAsync(employeeId, dto.AttendanceDate, cancellationToken);
        if (attendance == null) throw new BusinessException("Attendance record not found for the selected date.");

        var pendingExists = await repository.PendingRequestExistsAsync(employeeId, dto.AttendanceDate, cancellationToken);
        if (pendingExists) throw new BusinessException("A pending regularization request already exists for this attendance date.");

        var request = new AttendanceRegularization
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            AttendanceDate = dto.AttendanceDate,
            RequestedCheckIn = dto.RequestedCheckIn,
            RequestedCheckOut = dto.RequestedCheckOut,
            Reason = dto.Reason,
            Status = AttendanceRegularizationStatuses.Pending,
            CreatedAt = DateTime.Now
        };

        await repository.AddRegularizationAsync(request, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Create", nameof(AttendanceRegularization), request.Id, $"Attendance regularization requested for {dto.AttendanceDate}", cancellationToken);
        logger.LogInformation("Attendance regularization {RequestId} created by employee {EmployeeId}", request.Id, employeeId);

        request.Employee = employee;
        return MapToResponse(request);
    }

    public async Task<PagedResponse<AttendanceRegularizationResponseDto>> GetRequestsAsync(AttendanceRegularizationFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = repository.GetRegularizations();

        if (!userContextService.IsAdminOrHr())
        {
            var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
            if (employeeId == null) throw new NotFoundException("Employee profile not found.");
            query = query.Where(x => x.EmployeeId == employeeId.Value);
        }
        else if (filter.EmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == filter.EmployeeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Status)) query = query.Where(x => x.Status == filter.Status);
        if (filter.FromDate.HasValue) query = query.Where(x => x.AttendanceDate >= filter.FromDate.Value);
        if (filter.ToDate.HasValue) query = query.Where(x => x.AttendanceDate <= filter.ToDate.Value);

        query = ApplySorting(query, filter);

        var totalRecords = await query.CountAsync(cancellationToken);
        var requests = await query.Skip((filter.PageNumber - 1) * filter.PageSize)
                                  .Take(filter.PageSize)
                                  .ToListAsync(cancellationToken);

        return new PagedResponse<AttendanceRegularizationResponseDto>
        {
            Data = requests.Select(MapToResponse).ToList(),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize)
        };
    }

    public async Task<AttendanceRegularizationResponseDto> ApproveAsync(Guid regularizationId, ApproveAttendanceRegularizationDto dto, CancellationToken cancellationToken = default)
    {
        var request = await repository.GetRegularizationAsync(regularizationId, cancellationToken);
        if (request == null) throw new NotFoundException("Attendance regularization request not found.");
        if (request.Status != AttendanceRegularizationStatuses.Pending) throw new BusinessException("Only pending requests can be approved.");

        var attendance = await repository.GetAttendanceAsync(request.EmployeeId, request.AttendanceDate, cancellationToken);
        if (attendance == null) throw new NotFoundException("Attendance record not found.");

        attendance.CheckIn = request.RequestedCheckIn;
        attendance.CheckOut = request.RequestedCheckOut;

        if (attendance.CheckIn.HasValue && attendance.CheckOut.HasValue)
        {
            attendance.WorkingHours = Math.Round(Convert.ToDecimal((attendance.CheckOut.Value - attendance.CheckIn.Value).TotalHours), 2);
            attendance.Status = attendance.WorkingHours < AttendanceConstants.HalfDayThresholdHours ? AttendanceConstants.HalfDay : AttendanceConstants.Present;
        }

        repository.UpdateAttendance(attendance);

        request.Status = AttendanceRegularizationStatuses.Approved;
        request.HrComments = dto.HrComments;
        request.ReviewedAt = DateTime.Now;
        request.ReviewedBy = await userContextService.GetEmployeeIdAsync(cancellationToken);

        repository.UpdateRegularization(request);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Approve", nameof(AttendanceRegularization), request.Id, $"Attendance regularization approved for employee {request.EmployeeId}", cancellationToken);
        return MapToResponse(request);
    }

    public async Task<AttendanceRegularizationResponseDto> RejectAsync(Guid regularizationId, RejectAttendanceRegularizationDto dto, CancellationToken cancellationToken = default)
    {
        var request = await repository.GetRegularizationAsync(regularizationId, cancellationToken);
        if (request == null) throw new NotFoundException("Attendance regularization request not found.");
        if (request.Status != AttendanceRegularizationStatuses.Pending) throw new BusinessException("Only pending requests can be rejected.");

        request.Status = AttendanceRegularizationStatuses.Rejected;
        request.HrComments = dto.HrComments;
        request.ReviewedAt = DateTime.Now;
        request.ReviewedBy = await userContextService.GetEmployeeIdAsync(cancellationToken);

        repository.UpdateRegularization(request);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Reject", nameof(AttendanceRegularization), request.Id, $"Attendance regularization rejected for employee {request.EmployeeId}", cancellationToken);
        return MapToResponse(request);
    }

    private static AttendanceRegularizationResponseDto MapToResponse(AttendanceRegularization request) => new()
    {
        Id = request.Id,
        EmployeeId = request.EmployeeId,
        EmployeeCode = request.Employee?.EmployeeCode ?? string.Empty,
        EmployeeName = request.Employee == null ? string.Empty : $"{request.Employee.FirstName} {request.Employee.LastName}",
        AttendanceDate = request.AttendanceDate,
        RequestedCheckIn = request.RequestedCheckIn,
        RequestedCheckOut = request.RequestedCheckOut,
        Reason = request.Reason ?? string.Empty,
        Status = request.Status ?? string.Empty,
        HrComments = request.HrComments,
        ReviewedBy = request.ReviewedBy,
        ReviewedAt = request.ReviewedAt,
        CreatedAt = request.CreatedAt
    };

    private static IQueryable<AttendanceRegularization> ApplySorting(IQueryable<AttendanceRegularization> query, AttendanceRegularizationFilterDto filter)
    {
        return filter.SortBy?.ToLower() switch
        {
            "date" => filter.Descending ? query.OrderByDescending(x => x.AttendanceDate) : query.OrderBy(x => x.AttendanceDate),
            "status" => filter.Descending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            "createdat" => filter.Descending ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
            _ => query.OrderByDescending(x => x.CreatedAt)
        };
    }
}
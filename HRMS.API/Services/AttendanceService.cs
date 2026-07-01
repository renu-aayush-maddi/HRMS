using ClosedXML.Excel;
using HRMS.API.Common.Constants;
using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.Attendance;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository repository;
    private readonly IUserContextService userContextService;
    private readonly IEmployeeAccessResolver accessResolver;
    private readonly IAuditLogService auditLogService;
    private readonly INotificationService notificationService;
    private readonly ILogger<AttendanceService> logger;

    public AttendanceService(
        IAttendanceRepository repository,
        IUserContextService userContextService,
        IEmployeeAccessResolver accessResolver,
        IAuditLogService auditLogService,
        INotificationService notificationService,
        ILogger<AttendanceService> logger)
    {
        this.repository = repository;
        this.userContextService = userContextService;
        this.accessResolver = accessResolver;
        this.auditLogService = auditLogService;
        this.notificationService = notificationService;
        this.logger = logger;
    }

    public async Task CheckInAsync(CancellationToken cancellationToken = default)
    {
        var userId = userContextService.GetUserId();
        var employee = await repository.GetEmployeeByUserIdAsync(userId, cancellationToken);

        if (employee == null) throw new NotFoundException("Employee not found.");

        if (employee.EmploymentStatus == "Resigned" || employee.EmploymentStatus == "Terminated" || employee.EmploymentStatus == "Inactive")
            throw new BusinessException("Attendance actions are only allowed for active employees.");

        var existingAttendance = await repository.GetTodayAttendanceAsync(employee.Id, cancellationToken);
        if (existingAttendance != null) throw new BusinessException("You have already checked in today.");

        var now = DateTime.Now;
        var status = now.TimeOfDay > AttendanceConstants.LateThreshold.ToTimeSpan()
            ? AttendanceConstants.Late
            : AttendanceConstants.Present;

        var attendance = new AttendanceLog
        {
            Id = Guid.NewGuid(),
            EmployeeId = employee.Id,
            AttendanceDate = DateOnly.FromDateTime(now),
            CheckIn = now,
            Status = status,
            CreatedAt = now,
            CreatedBy = userId
        };

        await repository.AddAttendanceAsync(attendance, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("CheckIn", nameof(AttendanceLog), attendance.Id, $"Employee {employee.Id} checked in.", cancellationToken);
        logger.LogInformation("Employee {EmployeeId} checked in. AttendanceId: {AttendanceId}", employee.Id, attendance.Id);

        if (employee.Manager?.UserId != null)
        {
            notificationService.CreateNotification(
                employee.Manager.UserId.Value,
                "Attendance Check-In",
                $"{employee.FirstName} {employee.LastName} has checked in.");
        }
    }

    public async Task CheckOutAsync(CancellationToken cancellationToken = default)
    {
        var userId = userContextService.GetUserId();
        var employee = await repository.GetEmployeeByUserIdAsync(userId, cancellationToken);

        if (employee == null) throw new NotFoundException("Employee not found.");

        if (employee.EmploymentStatus == "Resigned" || employee.EmploymentStatus == "Terminated" || employee.EmploymentStatus == "Inactive")
            throw new BusinessException("Attendance actions are only allowed for active employees.");

        var attendance = await repository.GetTodayAttendanceAsync(employee.Id, cancellationToken);
        if (attendance == null) throw new NotFoundException("Check-in record not found.");
        if (attendance.CheckOut != null) throw new BusinessException("You have already checked out.");

        var now = DateTime.Now;
        if (attendance.CheckIn.HasValue && now <= attendance.CheckIn.Value)
            throw new BusinessException("Check-out time cannot be earlier than check-in time.");

        attendance.CheckOut = now;

        if (attendance.CheckIn.HasValue)
        {
            attendance.WorkingHours = Convert.ToDecimal((attendance.CheckOut.Value - attendance.CheckIn.Value).TotalHours);
            if (attendance.WorkingHours < AttendanceConstants.HalfDayThresholdHours)
                attendance.Status = AttendanceConstants.HalfDay;
        }

        attendance.UpdatedAt = now;
        attendance.UpdatedBy = userId;

        repository.UpdateAttendance(attendance);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("CheckOut", nameof(AttendanceLog), attendance.Id, $"Employee {employee.Id} checked out.", cancellationToken);
        logger.LogInformation("Employee {EmployeeId} checked out. AttendanceId: {AttendanceId}", employee.Id, attendance.Id);

        if (employee.Manager?.UserId != null)
        {
            notificationService.CreateNotification(
                employee.Manager.UserId.Value,
                "Attendance Check-Out",
                $"{employee.FirstName} {employee.LastName} has checked out.");
        }
    }

    public async Task<PagedResponse<AttendanceResponseDto>> GetAttendanceAsync(AttendanceFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = repository.GetAttendances();

        if (filter.EmployeeId.HasValue) query = query.Where(x => x.EmployeeId == filter.EmployeeId.Value);
        if (!string.IsNullOrWhiteSpace(filter.Status)) query = query.Where(x => x.Status == filter.Status);
        if (filter.FromDate.HasValue) query = query.Where(x => x.AttendanceDate >= filter.FromDate.Value);
        if (filter.ToDate.HasValue) query = query.Where(x => x.AttendanceDate <= filter.ToDate.Value);

        query = ApplySorting(query, filter);

        var totalRecords = await query.CountAsync(cancellationToken);
        var records = await query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToListAsync(cancellationToken);

        return new PagedResponse<AttendanceResponseDto>
        {
            Data = records.Select(MapToResponse).ToList(),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize)
        };
    }

    public async Task<PagedResponse<AttendanceResponseDto>> GetEmployeeAttendanceAsync(AttendanceFilterDto filter, CancellationToken cancellationToken = default)
    {
        var employeeId = await accessResolver.ResolveEmployeeIdAsync(filter.EmployeeId, cancellationToken);
        await accessResolver.ValidateEmployeeOwnershipAsync(employeeId, cancellationToken);

        filter.EmployeeId = employeeId;
        return await GetAttendanceAsync(filter, cancellationToken);
    }

    public async Task<byte[]> ExportAttendanceAsync(AttendanceFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = repository.GetAttendances();

        if (filter.EmployeeId.HasValue) query = query.Where(x => x.EmployeeId == filter.EmployeeId.Value);
        if (!string.IsNullOrWhiteSpace(filter.Status)) query = query.Where(x => x.Status == filter.Status);
        if (filter.FromDate.HasValue) query = query.Where(x => x.AttendanceDate >= filter.FromDate.Value);
        if (filter.ToDate.HasValue) query = query.Where(x => x.AttendanceDate <= filter.ToDate.Value);

        var attendances = await query.OrderByDescending(x => x.AttendanceDate).ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Attendance");

        worksheet.Cell(1, 1).Value = "Employee Code";
        worksheet.Cell(1, 2).Value = "Employee Name";
        worksheet.Cell(1, 3).Value = "Attendance Date";
        worksheet.Cell(1, 4).Value = "Check In";
        worksheet.Cell(1, 5).Value = "Check Out";
        worksheet.Cell(1, 6).Value = "Working Hours";
        worksheet.Cell(1, 7).Value = "Status";

        int row = 2;
        foreach (var attendance in attendances)
        {
            worksheet.Cell(row, 1).Value = attendance.Employee?.EmployeeCode ?? string.Empty;
            worksheet.Cell(row, 2).Value = attendance.Employee == null ? string.Empty : $"{attendance.Employee.FirstName} {attendance.Employee.LastName}";
            worksheet.Cell(row, 3).Value = attendance.AttendanceDate.ToString();
            worksheet.Cell(row, 4).Value = attendance.CheckIn?.ToString("g");
            worksheet.Cell(row, 5).Value = attendance.CheckOut?.ToString("g");
            worksheet.Cell(row, 6).Value = attendance.WorkingHours;
            worksheet.Cell(row, 7).Value = attendance.Status;
            row++;
        }

        worksheet.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        await auditLogService.LogAsync("Export", nameof(AttendanceLog), Guid.Empty, "Attendance exported.", cancellationToken);
        return stream.ToArray();
    }

    private static AttendanceResponseDto MapToResponse(AttendanceLog attendance) => new()
    {
        Id = attendance.Id,
        EmployeeId = attendance.EmployeeId ?? Guid.Empty,
        EmployeeCode = attendance.Employee?.EmployeeCode ?? string.Empty,
        EmployeeName = attendance.Employee == null? string.Empty: $"{attendance.Employee.FirstName} {attendance.Employee.LastName}",
        AttendanceDate = attendance.AttendanceDate,
        CheckIn = attendance.CheckIn,
        CheckOut = attendance.CheckOut,
        WorkingHours = attendance.WorkingHours,
        Status = attendance.Status ?? string.Empty,
        Remarks = attendance.Remarks
    };

    private static IQueryable<AttendanceLog> ApplySorting(IQueryable<AttendanceLog> query, AttendanceFilterDto filter)
    {
        return filter.SortBy?.ToLower() switch
        {
            "date" => filter.Descending ? query.OrderByDescending(x => x.AttendanceDate) : query.OrderBy(x => x.AttendanceDate),
            "employee" => filter.Descending
                ? query.OrderByDescending(x => x.Employee != null ? x.Employee.FirstName : string.Empty)
                : query.OrderBy(x => x.Employee != null ? x.Employee.FirstName : string.Empty),
            "status" => filter.Descending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            "workinghours" => filter.Descending ? query.OrderByDescending(x => x.WorkingHours) : query.OrderBy(x => x.WorkingHours),
            _ => query.OrderByDescending(x => x.AttendanceDate)
        };
    }
}
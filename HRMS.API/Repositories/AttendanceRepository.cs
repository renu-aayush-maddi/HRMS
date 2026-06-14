using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Attendance;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly AppDbContext context;

    public AttendanceRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<AttendanceLog?> GetTodayAttendanceAsync(Guid employeeId)
    {
        return await context.AttendanceLogs
            .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.AttendanceDate == DateOnly.FromDateTime(DateTime.Now));
    }

    public async Task<List<AttendanceLog>> GetAttendanceAsync(AttendanceQueryDto query, int skip, int take)
    {
        var attendance = context.AttendanceLogs
            .Include(a => a.Employee)
            .AsQueryable();

        if (query.EmployeeId.HasValue)
        {
            attendance = attendance.Where(a => a.EmployeeId == query.EmployeeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            attendance = attendance.Where(a => a.Status == query.Status);
        }

        if (query.FromDate.HasValue)
        {
            attendance = attendance.Where(a => a.AttendanceDate >= query.FromDate.Value);
        }

        if (query.ToDate.HasValue)
        {
            attendance = attendance.Where(a => a.AttendanceDate <= query.ToDate.Value);
        }

        attendance = ApplySorting(attendance, query);

        return await attendance
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> GetAttendanceCountAsync(AttendanceQueryDto query)
    {
        var attendance = context.AttendanceLogs.AsQueryable();

        if (query.EmployeeId.HasValue)
        {
            attendance = attendance.Where(a => a.EmployeeId == query.EmployeeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            attendance = attendance.Where(a => a.Status == query.Status);
        }

        if (query.FromDate.HasValue)
        {
            attendance = attendance.Where(a => a.AttendanceDate >= query.FromDate.Value);
        }

        if (query.ToDate.HasValue)
        {
            attendance = attendance.Where(a => a.AttendanceDate <= query.ToDate.Value);
        }

        return await attendance.CountAsync();
    }

    public async Task<List<AttendanceLog>> GetEmployeeAttendanceAsync(Guid employeeId)
    {
        return await context.AttendanceLogs
            .Include(a => a.Employee)
            .Where(a => a.EmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task AddAttendanceAsync(AttendanceLog attendance)
    {
        await context.AttendanceLogs.AddAsync(attendance);
    }

    public void UpdateAttendance(AttendanceLog attendance)
    {
        context.AttendanceLogs.Update(attendance);
    }

    public async Task<Employee?> GetEmployeeAsync(Guid employeeId)
    {
        return await context.Employees
            .FirstOrDefaultAsync(e => e.Id == employeeId);
    }

    public async Task<Employee?> GetEmployeeByUserIdAsync(Guid userId)
    {
        return await context.Employees
            .FirstOrDefaultAsync(e => e.UserId == userId);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    private IQueryable<AttendanceLog> ApplySorting(IQueryable<AttendanceLog> attendance, AttendanceQueryDto query)
    {
        var sortBy = query.SortBy?.ToLower();
        var direction = query.SortDirection?.ToLower();

        return (sortBy, direction) switch
        {
            ("attendancedate", "desc") => attendance.OrderByDescending(a => a.AttendanceDate),
            ("attendancedate", _) => attendance.OrderBy(a => a.AttendanceDate),
            ("checkin", "desc") => attendance.OrderByDescending(a => a.CheckIn),
            ("checkin", _) => attendance.OrderBy(a => a.CheckIn),
            ("status", "desc") => attendance.OrderByDescending(a => a.Status),
            ("status", _) => attendance.OrderBy(a => a.Status),
            _ => attendance.OrderByDescending(a => a.AttendanceDate)
        };
    }
}
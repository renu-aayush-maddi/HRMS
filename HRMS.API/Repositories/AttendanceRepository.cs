using HRMS.API.Data;
using HRMS.API.Interfaces;
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

    public async Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(x => x.Id == employeeId && !x.IsDeleted, cancellationToken);
    }

    public async Task<Employee?> GetEmployeeByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .Include(x => x.Manager)
            .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted, cancellationToken);
    }

    public async Task<AttendanceLog?> GetAttendanceAsync(Guid attendanceId, CancellationToken cancellationToken = default)
    {
        return await context.AttendanceLogs
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x => x.Id == attendanceId, cancellationToken);
    }

    public async Task<AttendanceLog?> GetTodayAttendanceAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.AttendanceLogs
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.AttendanceDate == DateOnly.FromDateTime(DateTime.Now), cancellationToken);
    }

    public IQueryable<AttendanceLog> GetAttendances()
    {
        return context.AttendanceLogs
            .Include(x => x.Employee)
            .AsNoTracking();
    }

    public async Task AddAttendanceAsync(AttendanceLog attendance, CancellationToken cancellationToken = default)
    {
        await context.AttendanceLogs.AddAsync(attendance, cancellationToken);
    }

    public void UpdateAttendance(AttendanceLog attendance)
    {
        context.AttendanceLogs.Update(attendance);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class AttendanceRegularizationRepository : IAttendanceRegularizationRepository
{
    private readonly AppDbContext context;

    public AttendanceRegularizationRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(x => x.Id == employeeId && !x.IsDeleted, cancellationToken);
    }

    public async Task<AttendanceLog?> GetAttendanceAsync(Guid employeeId, DateOnly attendanceDate, CancellationToken cancellationToken = default)
    {
        return await context.AttendanceLogs
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.AttendanceDate == attendanceDate, cancellationToken);
    }

    public async Task<AttendanceRegularization?> GetRegularizationAsync(Guid regularizationId, CancellationToken cancellationToken = default)
    {
        return await context.AttendanceRegularizations
            .Include(x => x.Employee)
            .Include(x => x.ReviewedByNavigation)
            .FirstOrDefaultAsync(x => x.Id == regularizationId, cancellationToken);
    }

    public IQueryable<AttendanceRegularization> GetRegularizations()
    {
        return context.AttendanceRegularizations
            .Include(x => x.Employee)
            .Include(x => x.ReviewedByNavigation)
            .AsNoTracking();
    }

    public async Task<bool> PendingRequestExistsAsync(Guid employeeId, DateOnly attendanceDate, CancellationToken cancellationToken = default)
    {
        return await context.AttendanceRegularizations
            .AnyAsync(x => x.EmployeeId == employeeId && x.AttendanceDate == attendanceDate && x.Status == "Pending", cancellationToken);
    }

    public async Task AddRegularizationAsync(AttendanceRegularization regularization, CancellationToken cancellationToken = default)
    {
        await context.AttendanceRegularizations.AddAsync(regularization, cancellationToken);
    }

    public void UpdateRegularization(AttendanceRegularization regularization)
    {
        context.AttendanceRegularizations.Update(regularization);
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
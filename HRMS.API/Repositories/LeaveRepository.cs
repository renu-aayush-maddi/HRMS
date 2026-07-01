using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class LeaveRepository : ILeaveRepository
{
    private readonly AppDbContext context;

    public LeaveRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .Include(x => x.Manager)
            .FirstOrDefaultAsync(x => x.Id == employeeId && !x.IsDeleted, cancellationToken);
    }

    public async Task<Employee?> GetEmployeeByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted, cancellationToken);
    }

    public async Task<LeaveRequest?> GetLeaveByIdAsync(Guid leaveId, CancellationToken cancellationToken = default)
    {
        return await context.LeaveRequests
            .Include(x => x.Employee)
                .ThenInclude(e => e.User)
                    .ThenInclude(u => u.Roles)
            .Include(x => x.Employee)
                .ThenInclude(e => e.Manager)
            .Include(x => x.LeaveType)
            .FirstOrDefaultAsync(x => x.Id == leaveId, cancellationToken);
    }

    public IQueryable<LeaveRequest> GetLeaves()
    {
        return context.LeaveRequests
            .AsNoTracking()
            .Include(x => x.Employee)
            .Include(x => x.LeaveType);
    }

    public async Task<EmployeeLeaveBalance?> GetLeaveBalanceAsync(Guid employeeId, Guid leaveTypeId, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeLeaveBalances
            .Include(x => x.LeaveType)
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.LeaveTypeId == leaveTypeId, cancellationToken);
    }

    public async Task<List<EmployeeLeaveBalance>> GetEmployeeLeaveBalancesAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeLeaveBalances
            .Include(x => x.LeaveType)
            .Where(x => x.EmployeeId == employeeId)
            .ToListAsync(cancellationToken);
    }

    public async Task<LeaveType?> GetLeaveTypeAsync(Guid leaveTypeId, CancellationToken cancellationToken = default)
    {
        return await context.LeaveTypes
            .FirstOrDefaultAsync(x => x.Id == leaveTypeId && x.IsActive == true, cancellationToken);
    }

    public async Task<bool> HasOverlappingLeaveAsync(Guid employeeId, DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default)
    {
        return await context.LeaveRequests
            .AnyAsync(x => x.EmployeeId == employeeId &&
                           (x.Status == "Pending" || x.Status == "Approved") &&
                           fromDate <= x.ToDate &&
                           toDate >= x.FromDate, cancellationToken);
    }

    public async Task<bool> IsManagerOfEmployeeAsync(Guid managerEmployeeId, Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .AnyAsync(x => x.Id == employeeId && x.ManagerId == managerEmployeeId, cancellationToken);
    }

    public async Task AddLeaveAsync(LeaveRequest leave, CancellationToken cancellationToken = default)
    {
        await context.LeaveRequests.AddAsync(leave, cancellationToken);
    }

    public void UpdateLeave(LeaveRequest leave)
    {
        context.LeaveRequests.Update(leave);
    }

    public void UpdateLeaveBalance(EmployeeLeaveBalance balance)
    {
        context.EmployeeLeaveBalances.Update(balance);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
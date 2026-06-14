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

    public async Task<LeaveRequest?> GetLeaveByIdAsync(Guid leaveId)
    {
        return await context.LeaveRequests
            .Include(l => l.Employee)
            .Include(l => l.LeaveType)
            .FirstOrDefaultAsync(l => l.Id == leaveId);
    }

    public async Task<List<LeaveRequest>> GetAllLeavesAsync()
    {
        return await context.LeaveRequests
            .Include(l => l.Employee)
            .Include(l => l.LeaveType)
            .ToListAsync();
    }

    public async Task<List<LeaveRequest>> GetEmployeeLeavesAsync(Guid employeeId)
    {
        return await context.LeaveRequests
            .Include(l => l.Employee)
            .Include(l => l.LeaveType)
            .Where(l => l.EmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task AddLeaveAsync(LeaveRequest leave)
    {
        await context.LeaveRequests.AddAsync(leave);
    }

    public void UpdateLeave(LeaveRequest leave)
    {
        context.LeaveRequests.Update(leave);
    }

    public async Task<EmployeeLeaveBalance?> GetLeaveBalanceAsync(Guid employeeId, Guid leaveTypeId)
    {
        return await context.EmployeeLeaveBalances
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.LeaveTypeId == leaveTypeId);
    }

    public async Task<LeaveType?> GetLeaveTypeAsync(Guid leaveTypeId)
    {
        return await context.LeaveTypes
            .FirstOrDefaultAsync(x => x.Id == leaveTypeId);
    }

    public void UpdateLeaveBalance(EmployeeLeaveBalance balance)
    {
        context.EmployeeLeaveBalances.Update(balance);
    }

    public async Task<bool> HasOverlappingLeaveAsync(Guid employeeId, DateOnly fromDate, DateOnly toDate)
    {
        return await context.LeaveRequests
            .AnyAsync(l => l.EmployeeId == employeeId && 
                          (l.Status == "Pending" || l.Status == "Approved") && 
                           fromDate <= l.ToDate && 
                           toDate >= l.FromDate);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
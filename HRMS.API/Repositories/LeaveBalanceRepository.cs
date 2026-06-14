using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class LeaveBalanceRepository : ILeaveBalanceRepository
{
    private readonly AppDbContext context;

    public LeaveBalanceRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<Employee?> GetEmployeeAsync(Guid employeeId)
    {
        return await context.Employees
            .FirstOrDefaultAsync(x => x.Id == employeeId);
    }

    public async Task<LeaveType?> GetLeaveTypeAsync(Guid leaveTypeId)
    {
        return await context.LeaveTypes
            .FirstOrDefaultAsync(x => x.Id == leaveTypeId);
    }

    public async Task<EmployeeLeaveBalance?> GetBalanceAsync(Guid employeeId,Guid leaveTypeId)
    {
        return await context.EmployeeLeaveBalances
            .FirstOrDefaultAsync(x =>
                x.EmployeeId == employeeId &&
                x.LeaveTypeId == leaveTypeId);
    }

    public async Task<List<EmployeeLeaveBalance>> GetAllBalancesAsync()
    {
        return await context.EmployeeLeaveBalances
            .Include(x => x.Employee)
            .Include(x => x.LeaveType)
            .ToListAsync();
    }

    public async Task<List<EmployeeLeaveBalance>> GetEmployeeBalancesAsync(Guid employeeId)
    {
        return await context.EmployeeLeaveBalances
            .Include(x => x.Employee)
            .Include(x => x.LeaveType)
            .Where(x => x.EmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task AddBalanceAsync(EmployeeLeaveBalance balance)
    {
        await context.EmployeeLeaveBalances.AddAsync(balance);
    }

    public void UpdateBalance(EmployeeLeaveBalance balance)
    {
        context.EmployeeLeaveBalances.Update(balance);
    }

    public async Task<List<LeaveType>> GetActiveLeaveTypesAsync()
    {
        return await context.LeaveTypes
            .Where(x => x.IsActive == true)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
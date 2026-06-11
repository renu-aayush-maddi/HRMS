using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class LeaveBalanceRepository: ILeaveBalanceRepository
{
    private readonly AppDbContext context;

    public LeaveBalanceRepository(AppDbContext context)
    {
        this.context = context;
    }

    public Employee? GetEmployee(Guid employeeId)
    {
        return context.Employees.FirstOrDefault(x => x.Id == employeeId);
    }

    public LeaveType? GetLeaveType(Guid leaveTypeId)
    {
        return context.LeaveTypes
            .FirstOrDefault(x =>
                x.Id == leaveTypeId);
    }

    public EmployeeLeaveBalance? GetBalance(Guid employeeId,Guid leaveTypeId)
    {
        return context.EmployeeLeaveBalances
            .FirstOrDefault(x =>
                x.EmployeeId == employeeId
                &&
                x.LeaveTypeId == leaveTypeId);
    }

    public List<EmployeeLeaveBalance> GetAllBalances()
    {
        return context.EmployeeLeaveBalances
            .Include(x => x.Employee)
            .Include(x => x.LeaveType)
            .ToList();
    }

    public List<EmployeeLeaveBalance> GetEmployeeBalances(Guid employeeId)
    {
        return context.EmployeeLeaveBalances
            .Include(x => x.Employee)
            .Include(x => x.LeaveType)
            .Where(x =>
                x.EmployeeId == employeeId)
            .ToList();
    }

    public void AddBalance(EmployeeLeaveBalance balance)
    {
        context.EmployeeLeaveBalances.Add(balance);
    }

    public void UpdateBalance(EmployeeLeaveBalance balance)
    {
        context.EmployeeLeaveBalances
            .Update(balance);
    }

    public List<LeaveType> GetActiveLeaveTypes()
    {
        return context.LeaveTypes
            .Where(x => x.IsActive == true)
            .ToList();
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}
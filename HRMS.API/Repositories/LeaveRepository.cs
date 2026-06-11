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

    public Employee? GetEmployee(Guid employeeId)
    {
        return context.Employees.FirstOrDefault(e => e.Id == employeeId);
    }

    public LeaveRequest? GetLeaveById(Guid leaveId)
    {
        return context.LeaveRequests
            .Include(l => l.Employee)
            .Include(l => l.LeaveType)
            .FirstOrDefault(l => l.Id == leaveId);
    }

    public List<LeaveRequest> GetAllLeaves()
    {
        return context.LeaveRequests
            .Include(l => l.Employee)
            .Include(x => x.LeaveType)
            .ToList();
    }


    public Employee? GetEmployeeByUserId(Guid userId)
    {
        return context.Employees
            .FirstOrDefault(e =>
                e.UserId == userId);
    }

    public List<LeaveRequest> GetEmployeeLeaves(Guid employeeId)
    {
        return context.LeaveRequests
            .Include(l => l.Employee)
            .Include(l => l.LeaveType)
            .Where(l => l.EmployeeId == employeeId)
            .ToList();
    }

    public void AddLeave(LeaveRequest leave)
    {
        context.LeaveRequests.Add(leave);
    }

    public void UpdateLeave(LeaveRequest leave)
    {
        context.LeaveRequests.Update(leave);
    }



    public EmployeeLeaveBalance? GetLeaveBalance(Guid employeeId,Guid leaveTypeId)
    {
        return context.EmployeeLeaveBalances
            .FirstOrDefault(x =>
                x.EmployeeId == employeeId &&
                x.LeaveTypeId == leaveTypeId);
    }

    public LeaveType? GetLeaveType(
        Guid leaveTypeId)
    {
        return context.LeaveTypes
            .FirstOrDefault(x =>
                x.Id == leaveTypeId);
    }

    public void UpdateLeaveBalance(
        EmployeeLeaveBalance balance)
    {
        context.EmployeeLeaveBalances
            .Update(balance);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }


    public bool HasOverlappingLeave(Guid employeeId,DateOnly fromDate,DateOnly toDate)
    {
        return context.LeaveRequests.Any(l =>
            l.EmployeeId == employeeId &&
            l.Status != "Rejected" &&
            fromDate <= l.ToDate &&
            toDate >= l.FromDate);
    }
}
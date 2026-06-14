using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface ILeaveBalanceRepository
{
    Task<Employee?> GetEmployeeAsync(Guid employeeId);

    Task<LeaveType?> GetLeaveTypeAsync(Guid leaveTypeId);

    Task<EmployeeLeaveBalance?> GetBalanceAsync(Guid employeeId, Guid leaveTypeId);

    Task<List<EmployeeLeaveBalance>> GetAllBalancesAsync();

    Task<List<EmployeeLeaveBalance>> GetEmployeeBalancesAsync(Guid employeeId);

    Task AddBalanceAsync(EmployeeLeaveBalance balance);

    void UpdateBalance(EmployeeLeaveBalance balance);

    Task<List<LeaveType>> GetActiveLeaveTypesAsync();

    Task SaveChangesAsync();
}
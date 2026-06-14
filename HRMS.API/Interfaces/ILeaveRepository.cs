using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface ILeaveRepository
{
    Task<Employee?> GetEmployeeAsync(Guid employeeId);

    Task<Employee?> GetEmployeeByUserIdAsync(Guid userId);

    Task<LeaveRequest?> GetLeaveByIdAsync(Guid leaveId);

    Task<List<LeaveRequest>> GetAllLeavesAsync();

    Task<List<LeaveRequest>> GetEmployeeLeavesAsync(
        Guid employeeId);

    Task AddLeaveAsync(LeaveRequest leave);

    void UpdateLeave(LeaveRequest leave);

    Task<EmployeeLeaveBalance?> GetLeaveBalanceAsync(Guid employeeId,Guid leaveTypeId);

    Task<LeaveType?> GetLeaveTypeAsync(Guid leaveTypeId);

    void UpdateLeaveBalance(EmployeeLeaveBalance balance);

    Task<bool> HasOverlappingLeaveAsync(
        Guid employeeId,
        DateOnly fromDate,
        DateOnly toDate);

    Task SaveChangesAsync();
}
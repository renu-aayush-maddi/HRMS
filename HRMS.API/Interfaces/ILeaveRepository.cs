using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface ILeaveRepository
{
    Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<LeaveRequest?> GetLeaveByIdAsync(Guid leaveId, CancellationToken cancellationToken = default);

    IQueryable<LeaveRequest> GetLeaves();

    Task<EmployeeLeaveBalance?> GetLeaveBalanceAsync(Guid employeeId, Guid leaveTypeId, CancellationToken cancellationToken = default);

    Task<List<EmployeeLeaveBalance>> GetEmployeeLeaveBalancesAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<LeaveType?> GetLeaveTypeAsync(Guid leaveTypeId, CancellationToken cancellationToken = default);

    Task<bool> HasOverlappingLeaveAsync(Guid employeeId, DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default);

    Task<bool> IsManagerOfEmployeeAsync(Guid managerEmployeeId, Guid employeeId, CancellationToken cancellationToken = default);

    Task AddLeaveAsync(LeaveRequest leave, CancellationToken cancellationToken = default);

    void UpdateLeave(LeaveRequest leave);

    void UpdateLeaveBalance(EmployeeLeaveBalance balance);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface ILeaveRepository
{
    Employee? GetEmployee(Guid employeeId);

    Employee? GetEmployeeByUserId(Guid userId);

    LeaveRequest? GetLeaveById(Guid leaveId);

    List<LeaveRequest> GetAllLeaves();

    List<LeaveRequest> GetEmployeeLeaves(Guid employeeId);

    void AddLeave(LeaveRequest leave);

    void UpdateLeave(LeaveRequest leave);


    EmployeeLeaveBalance? GetLeaveBalance(Guid employeeId,Guid leaveTypeId);

    LeaveType? GetLeaveType(Guid leaveTypeId);

    void UpdateLeaveBalance(EmployeeLeaveBalance balance);

    bool HasOverlappingLeave(Guid employeeId,DateOnly fromDate,DateOnly toDate);

    void SaveChanges();
}
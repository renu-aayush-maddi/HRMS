using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface ILeaveBalanceRepository
{
    Employee? GetEmployee(Guid employeeId);

    LeaveType? GetLeaveType(Guid leaveTypeId);

    EmployeeLeaveBalance? GetBalance(Guid employeeId,Guid leaveTypeId);

    List<EmployeeLeaveBalance> GetAllBalances();

    List<EmployeeLeaveBalance> GetEmployeeBalances(Guid employeeId);

    void AddBalance(EmployeeLeaveBalance balance);

    void UpdateBalance(EmployeeLeaveBalance balance);

    List<LeaveType> GetActiveLeaveTypes();

    void SaveChanges();
}
using HRMS.API.Models.DTOs.LeaveBalance;

namespace HRMS.API.Interfaces;

public interface ILeaveBalanceService
{
    void Allocate(AllocateLeaveBalanceDto dto);

    List<LeaveBalanceResponseDto> GetAllBalances();

    List<LeaveBalanceResponseDto> GetEmployeeBalances(Guid employeeId);

    void AllocateDefaultBalances(Guid employeeId);
}
using HRMS.API.Models.DTOs.LeaveBalance;

namespace HRMS.API.Interfaces;

public interface ILeaveBalanceService
{
    Task AllocateAsync(AllocateLeaveBalanceDto dto);

    Task<List<LeaveBalanceResponseDto>> GetAllBalancesAsync();

    Task<List<LeaveBalanceResponseDto>> GetEmployeeBalancesAsync(Guid employeeId);

    Task AllocateDefaultBalancesAsync(Guid employeeId);
}
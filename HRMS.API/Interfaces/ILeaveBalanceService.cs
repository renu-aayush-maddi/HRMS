using HRMS.API.Models.DTOs.LeaveBalance;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.API.Interfaces;

public interface ILeaveBalanceService
{
    Task AllocateAsync(AllocateLeaveBalanceDto dto, CancellationToken cancellationToken = default);

    Task<List<LeaveBalanceResponseDto>> GetAllBalancesAsync(CancellationToken cancellationToken = default);

    Task<List<LeaveBalanceResponseDto>> GetEmployeeBalancesAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task AllocateDefaultBalancesAsync(Guid employeeId, CancellationToken cancellationToken = default);
}
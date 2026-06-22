using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.Leave;

namespace HRMS.API.Interfaces;

public interface ILeaveService
{
    Task ApplyLeaveAsync(ApplyLeaveDto dto, CancellationToken cancellationToken = default);

    Task<PagedResponse<LeaveResponseDto>> GetLeavesAsync(LeaveFilterDto filter, CancellationToken cancellationToken = default);

    Task<PagedResponse<LeaveResponseDto>> GetMyLeavesAsync(LeaveFilterDto filter, CancellationToken cancellationToken = default);

    Task<LeaveDetailsDto> GetLeaveAsync(Guid leaveId, CancellationToken cancellationToken = default);

    Task ApproveLeaveAsync(Guid leaveId, LeaveActionDto dto, CancellationToken cancellationToken = default);

    Task RejectLeaveAsync(Guid leaveId, LeaveActionDto dto, CancellationToken cancellationToken = default);

    Task WithdrawLeaveAsync(Guid leaveId, CancellationToken cancellationToken = default);

    Task CancelLeaveAsync(Guid leaveId, CancellationToken cancellationToken = default);

    Task<List<LeaveBalanceDto>> GetMyLeaveBalancesAsync(CancellationToken cancellationToken = default);
}
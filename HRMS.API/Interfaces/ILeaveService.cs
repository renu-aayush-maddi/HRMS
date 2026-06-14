using HRMS.API.Models.DTOs.Leave;

namespace HRMS.API.Interfaces;

public interface ILeaveService
{
    Task ApplyLeaveAsync(ApplyLeaveDto dto, Guid userId, string role);

    Task<List<LeaveResponseDto>> GetMyLeavesAsync(Guid userId);

    Task<List<LeaveResponseDto>> GetEmployeeLeavesAsync(Guid employeeId);

    Task ApproveLeaveAsync(Guid leaveId, Guid userId, string role, LeaveActionDto dto);

    Task RejectLeaveAsync(Guid leaveId, Guid userId, string role, LeaveActionDto dto);

    Task<List<LeaveResponseDto>> GetAllLeavesAsync();
}
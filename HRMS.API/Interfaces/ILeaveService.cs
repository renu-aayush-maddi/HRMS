using HRMS.API.Models.DTOs.Leave;

namespace HRMS.API.Interfaces;

public interface ILeaveService
{
    
void ApplyLeave(ApplyLeaveDto dto, Guid userId, string role);

List<LeaveResponseDto> GetMyLeaves(Guid userId);

List<LeaveResponseDto> GetEmployeeLeaves(Guid employeeId);

void ApproveLeave(Guid leaveId, Guid userId, string role, LeaveActionDto dto);

void RejectLeave(Guid leaveId, Guid userId, string role, LeaveActionDto dto);

List<LeaveResponseDto> GetAllLeaves();

}
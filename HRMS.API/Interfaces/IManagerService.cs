using HRMS.API.Models.DTOs.Manager;

using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IManagerService
{
    ManagerDashboardDto GetDashboard(
        Guid managerUserId);

    List<TeamMemberDto> GetTeamMembers(
        Guid managerUserId);

    TeamMemberDto GetTeamMember(
        Guid managerUserId,
        Guid employeeId);

    List<TeamAttendanceDto> GetTeamAttendance(Guid managerUserId);

    List<LateEmployeeDto> GetLateEmployees(Guid managerUserId);

    List<PendingLeaveDto> GetPendingLeaveRequests( Guid managerUserId);

    void AddPerformanceReview(Guid managerUserId, AddPerformanceReviewDto dto);

    List<PerformanceReviewDto> GetPerformanceReviews(Guid managerUserId);

    List<PerformanceReviewDto> GetEmployeePerformanceReviews(Guid managerUserId, Guid employeeId);

    EligibleEmployeesResponseDto GetEligibleEmployees(Guid managerUserId, string? search, int page, int pageSize);

    void AddTeamMember(Guid managerUserId, Guid employeeId);

    void RemoveTeamMember(Guid managerUserId, Guid employeeId);
}
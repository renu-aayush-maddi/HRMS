using HRMS.API.Models.DTOs.Manager;
using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IManagerRepository
{
    Employee? GetEmployeeByUserId(
        Guid userId);

    ManagerDashboardDto GetDashboard(
        Guid managerEmployeeId);

    List<Employee> GetTeamMembers(
        Guid managerEmployeeId);

    Employee? GetTeamMember(
        Guid managerEmployeeId,
        Guid employeeId);

    List<AttendanceLog> GetTeamAttendance(
    Guid managerEmployeeId);

    List<AttendanceLog> GetLateEmployees(Guid managerEmployeeId);
    List<LeaveRequest> GetPendingLeaveRequests(Guid managerEmployeeId);

    List<PerformanceReview> GetPerformanceReviews(Guid managerEmployeeId);

    List<PerformanceReview> GetEmployeePerformanceReviews(Guid managerEmployeeId,Guid employeeId);

    void AddPerformanceReview(PerformanceReview review);

    Employee? GetEmployeeById(Guid id);

    (List<Employee> Employees, int TotalCount) GetEligibleEmployees(Guid managerEmployeeId, string? search, int page, int pageSize);

    PerformanceCycle? GetPerformanceCycle(Guid id);

    void SaveChanges();
}
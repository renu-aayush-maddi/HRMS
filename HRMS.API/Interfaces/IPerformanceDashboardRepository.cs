using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.Dashboard;

namespace HRMS.API.Interfaces;

public interface IPerformanceDashboardRepository
{
    Employee? GetEmployeeByUserId(Guid userId);

    int GetTeamMembersCount(Guid managerId);

    int GetTotalGoals(Guid managerId);

    int GetCompletedGoals(Guid managerId);

    decimal GetAverageRating(Guid managerId,Guid cycleId);

    string? GetTopPerformer(Guid managerId,Guid cycleId);



    int GetTotalEmployees();

int GetTotalReviews(
    Guid cycleId);

decimal GetCompanyAverageRating(
    Guid cycleId);

decimal GetReviewCompletionPercentage(
    Guid cycleId);

List<EmployeePerformanceDto>
    GetTopPerformers(
        Guid cycleId);

List<EmployeePerformanceDto>
    GetLowestPerformers(
        Guid cycleId);

List<DepartmentRatingDto>
    GetDepartmentRatings(
        Guid cycleId);
    
}
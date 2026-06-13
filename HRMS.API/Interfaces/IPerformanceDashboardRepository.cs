using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.Dashboard;

namespace HRMS.API.Interfaces;

public interface IPerformanceDashboardRepository
{
    Employee? GetEmployeeByUserId(Guid userId);

    int GetTeamMembersCount(Guid managerId);

    int GetTotalGoals(Guid managerId);

    int GetCompletedGoals(Guid managerId);

    decimal GetAverageRating(Guid managerId);

    string? GetTopPerformer(Guid managerId);



    int GetTotalEmployees();

    int GetTotalReviews();

    decimal GetCompanyAverageRating();

    decimal GetReviewCompletionPercentage();

    List<EmployeePerformanceDto> GetTopPerformers();

    List<EmployeePerformanceDto> GetLowestPerformers();

    List<DepartmentRatingDto> GetDepartmentRatings();

    
}
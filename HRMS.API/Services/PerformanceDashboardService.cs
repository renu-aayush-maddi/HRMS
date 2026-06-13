using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Dashboard;

namespace HRMS.API.Services;

public class PerformanceDashboardService
    : IPerformanceDashboardService
{
    private readonly
        IPerformanceDashboardRepository
        repository;

    public PerformanceDashboardService(
        IPerformanceDashboardRepository repository)
    {
        this.repository = repository;
    }

    public ManagerDashboardDto
        GetManagerDashboard(
            Guid managerUserId,
            Guid cycleId)
    {
        var manager =
            repository.GetEmployeeByUserId(
                managerUserId);

        if (manager == null)
        {
            throw new NotFoundException(
                "Manager not found");
        }

        var totalGoals =
            repository.GetTotalGoals(
                manager.Id);

        var completedGoals =
            repository.GetCompletedGoals(
                manager.Id);

        return new ManagerDashboardDto
        {
            TeamMembers =
                repository.GetTeamMembersCount(
                    manager.Id),

            TotalGoals =
                totalGoals,

            CompletedGoals =
                completedGoals,

            PendingGoals =
                totalGoals -
                completedGoals,

            AverageRating =
                repository.GetAverageRating(
                    manager.Id,
                    cycleId),

            TopPerformer =
                repository.GetTopPerformer(
                    manager.Id,
                    cycleId)
                ?? "N/A"
        };
    }

    public HrDashboardDto
        GetHrDashboard(
            Guid cycleId)
    {
        return new HrDashboardDto
        {
            TotalEmployees =
                repository.GetTotalEmployees(),

            TotalReviews =
                repository.GetTotalReviews(
                    cycleId),

            AverageRating =
                repository.GetCompanyAverageRating(
                    cycleId),

            ReviewCompletionPercentage =
                repository.GetReviewCompletionPercentage(
                    cycleId),

            TopPerformers =
                repository.GetTopPerformers(
                    cycleId),

            LowestPerformers =
                repository.GetLowestPerformers(
                    cycleId),

            DepartmentRatings =
                repository.GetDepartmentRatings(
                    cycleId)
        };
    }
}
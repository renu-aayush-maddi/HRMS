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
            Guid managerUserId)
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
                    manager.Id),

            TopPerformer =
                repository.GetTopPerformer(
                    manager.Id)
                ?? "N/A"
        };
    }
    public HrDashboardDto GetHrDashboard()
{
    return new HrDashboardDto
    {
        TotalEmployees =
            repository.GetTotalEmployees(),

        TotalReviews =
            repository.GetTotalReviews(),

        AverageRating =
            repository.GetCompanyAverageRating(),

        ReviewCompletionPercentage =
            repository.GetReviewCompletionPercentage(),

        TopPerformers =
            repository.GetTopPerformers(),

        LowestPerformers =
            repository.GetLowestPerformers(),

        DepartmentRatings =
            repository.GetDepartmentRatings()
    };
}
}
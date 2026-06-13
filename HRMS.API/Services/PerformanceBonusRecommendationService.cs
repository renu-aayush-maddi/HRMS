using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.PerformanceBonusRecommendation;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

public class PerformanceBonusRecommendationService
    : IPerformanceBonusRecommendationService
{
    private readonly
        IPerformanceBonusRecommendationRepository
        repository;

    public PerformanceBonusRecommendationService(
        IPerformanceBonusRecommendationRepository repository)
    {
        this.repository = repository;
    }

    public void GenerateRecommendations(Guid cycleId)
    {


    var cycle = repository.GetCycle(cycleId);

    if(cycle == null)
    {
        throw new NotFoundException(
            "Performance cycle not found");
    }
        var employees =
            repository
            .GetEmployeesWithReviews(cycleId);

        foreach(var employee in employees)
        {

        
        if(repository.HasPendingRecommendation(employee.Id))
            {
                continue;
            }

            var rating =
                repository
                .GetAverageRating(
                    employee.Id,cycleId);

            var rule =
                repository
                .GetMatchingRule(
                    rating);

            if(rule == null)
            {
                continue;
            }

            if(rule.BonusPercentage <= 0)
            {
                continue;
            }

            var amount =
                (employee.Salary ?? 0m) *
                rule.BonusPercentage /
                100m;

            repository.AddRecommendation(
                new PerformanceBonusRecommendation
{
    Id = Guid.NewGuid(),

    EmployeeId =
        employee.Id,

    PerformanceCycleId =
        cycleId,

    AverageRating =
        rating,

    RecommendedPercentage =
        rule.BonusPercentage,

    RecommendedAmount =
        amount,

    Status =
        "Pending",

    CreatedAt =
        DateTime.Now
});
        }

        repository.SaveChanges();
    }

    public List<
        PerformanceBonusRecommendationResponseDto>
        GetRecommendations()
    {
        return repository
            .GetRecommendations()
            .Select(x =>
                new PerformanceBonusRecommendationResponseDto
                {
                    Id = x.Id,

                    EmployeeId =
                        x.EmployeeId,

                    EmployeeName =
                        x.Employee.FirstName +
                        " " +
                        x.Employee.LastName,

                    AverageRating =
                        x.AverageRating,

                    RecommendedPercentage =
                        x.RecommendedPercentage,

                    RecommendedAmount =
                        x.RecommendedAmount,

                    ApprovedAmount =
                        x.ApprovedAmount,

                    Status =
                        x.Status,

                    Remarks =
                        x.Remarks
                })
            .ToList();
    }

    public void UpdateRecommendation(
        Guid id,
        UpdatePerformanceBonusRecommendationDto dto)
    {
        var recommendation =
            repository.GetRecommendation(
                id);

        if(recommendation == null)
        {
            throw new NotFoundException(
                "Recommendation not found");
        }

        recommendation.ApprovedAmount =
            dto.ApprovedAmount;

        recommendation.Remarks =
            dto.Remarks;

        repository.SaveChanges();
    }

    public void ApproveRecommendation(
        Guid id)
    {
        var recommendation =
            repository.GetRecommendation(
                id);

        if(recommendation == null)
        {
            throw new NotFoundException(
                "Recommendation not found");
        }

        recommendation.Status =
            "Approved";

        repository.AddBonus(
            new Bonuse
            {
                Id = Guid.NewGuid(),

                EmployeeId =
                    recommendation.EmployeeId,

                Amount =
                    recommendation
                    .ApprovedAmount
                    ??
                    recommendation
                    .RecommendedAmount,

                BonusMonth =
                    DateTime.Today.Month,

                BonusYear =
                    DateTime.Today.Year,

                Status =
                    "Approved",

                IsProcessed =
                    false,

                Reason =
                    "Performance Bonus"
            });

        repository.SaveChanges();
    }

    public void RejectRecommendation(
        Guid id)
    {
        var recommendation =
            repository.GetRecommendation(
                id);

        if(recommendation == null)
        {
            throw new NotFoundException(
                "Recommendation not found");
        }

        recommendation.Status =
            "Rejected";

        repository.SaveChanges();
    }


    public PerformanceBonusRecommendationResponseDto
    GetRecommendation(Guid id)
{
    var recommendation =
        repository.GetRecommendation(id);

    if(recommendation == null)
    {
        throw new NotFoundException(
            "Recommendation not found");
    }

    return new PerformanceBonusRecommendationResponseDto
    {
        Id = recommendation.Id,

        EmployeeId =
            recommendation.EmployeeId,

        EmployeeName =
            recommendation.Employee.FirstName +
            " " +
            recommendation.Employee.LastName,

        AverageRating =
            recommendation.AverageRating,

        RecommendedPercentage =
            recommendation.RecommendedPercentage,

        RecommendedAmount =
            recommendation.RecommendedAmount,

        ApprovedAmount =
            recommendation.ApprovedAmount,

        Status =
            recommendation.Status,

        Remarks =
            recommendation.Remarks
    };
}
}
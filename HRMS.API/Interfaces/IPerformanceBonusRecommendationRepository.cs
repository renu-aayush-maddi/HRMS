using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IPerformanceBonusRecommendationRepository
{
    List<Employee> GetEmployeesWithReviews(Guid cycleId);

    decimal GetAverageRating(Guid employeeId,Guid cycleId);

    PerformanceBonusRule? GetMatchingRule(
        decimal rating);

    void AddRecommendation(
        PerformanceBonusRecommendation recommendation);

    List<PerformanceBonusRecommendation>
        GetRecommendations();

    PerformanceBonusRecommendation?
        GetRecommendation(
            Guid id);

    Employee? GetEmployee(
        Guid employeeId);

    void AddBonus(
        Bonuse bonus);

    void SaveChanges();

    bool HasPendingRecommendation(
    Guid employeeId);

    PerformanceCycle? GetCycle(
    Guid cycleId);
}
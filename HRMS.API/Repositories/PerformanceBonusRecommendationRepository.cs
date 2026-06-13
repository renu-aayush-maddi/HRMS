using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class PerformanceBonusRecommendationRepository
    : IPerformanceBonusRecommendationRepository
{
    private readonly AppDbContext context;

    public PerformanceBonusRecommendationRepository(
        AppDbContext context)
    {
        this.context = context;
    }

    public List<Employee> GetEmployeesWithReviews(
    Guid cycleId)
{
    return context.PerformanceReviews
        .Include(r => r.Employee)
        .Where(r =>
            r.PerformanceCycleId ==
            cycleId)
        .Select(r => r.Employee!)
        .Distinct()
        .ToList();
}

public PerformanceCycle? GetCycle(Guid cycleId)
{
    return context.PerformanceCycles
        .FirstOrDefault(
            x => x.Id == cycleId);
}

public decimal GetAverageRating(
    Guid employeeId,
    Guid cycleId)
{
    var reviews =
        context.PerformanceReviews
        .Where(r =>
            r.EmployeeId ==
            employeeId
            &&
            r.PerformanceCycleId ==
            cycleId);

    if(!reviews.Any())
    {
        return 0;
    }

    return Math.Round(
        reviews.Average(
            r => r.Rating ?? 0),
        2);
}

    public PerformanceBonusRule?
        GetMatchingRule(
            decimal rating)
    {
        return context
            .PerformanceBonusRules
            .FirstOrDefault(r =>
                r.IsActive == true
                &&
                rating >= r.MinRating
                &&
                rating <= r.MaxRating);
    }

    public void AddRecommendation(
        PerformanceBonusRecommendation recommendation)
    {
        context
            .PerformanceBonusRecommendations
            .Add(recommendation);
    }

    public List<PerformanceBonusRecommendation>
        GetRecommendations()
    {
        return context
            .PerformanceBonusRecommendations
            .Include(x => x.Employee)
            .OrderByDescending(
                x => x.CreatedAt)
            .ToList();
    }

    public PerformanceBonusRecommendation?
        GetRecommendation(Guid id)
    {
        return context
            .PerformanceBonusRecommendations
            .Include(x => x.Employee)
            .FirstOrDefault(
                x => x.Id == id);
    }

    public Employee? GetEmployee(
        Guid employeeId)
    {
        return context.Employees
            .FirstOrDefault(
                e => e.Id == employeeId);
    }

    public void AddBonus(
        Bonuse bonus)
    {
        context.Bonuses.Add(
            bonus);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }

    public bool HasPendingRecommendation(
    Guid employeeId)
{
    return context
        .PerformanceBonusRecommendations
        .Any(x =>
            x.EmployeeId == employeeId
            &&
            x.Status == "Pending");
}
}
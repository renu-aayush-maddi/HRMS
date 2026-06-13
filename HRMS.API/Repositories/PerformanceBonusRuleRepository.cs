using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;

namespace HRMS.API.Repositories;

public class PerformanceBonusRuleRepository
    : IPerformanceBonusRuleRepository
{
    private readonly AppDbContext context;

    public PerformanceBonusRuleRepository(
        AppDbContext context)
    {
        this.context = context;
    }

    public List<PerformanceBonusRule>
        GetAll()
    {
        return context
            .PerformanceBonusRules
            .OrderBy(
                x => x.MinRating)
            .ToList();
    }

    public PerformanceBonusRule?
        GetById(Guid id)
    {
        return context
            .PerformanceBonusRules
            .FirstOrDefault(
                x => x.Id == id);
    }

    public void Add(
        PerformanceBonusRule rule)
    {
        context
            .PerformanceBonusRules
            .Add(rule);
    }

    public void Update(
        PerformanceBonusRule rule)
    {
        context
            .PerformanceBonusRules
            .Update(rule);
    }

    public void Delete(
        PerformanceBonusRule rule)
    {
        context
            .PerformanceBonusRules
            .Remove(rule);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}
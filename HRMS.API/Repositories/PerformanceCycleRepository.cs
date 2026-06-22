using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;

namespace HRMS.API.Repositories;

public class PerformanceCycleRepository : IPerformanceCycleRepository
{
    private readonly AppDbContext context;

    public PerformanceCycleRepository(AppDbContext context)
    {
        this.context = context;
    }

    public List<PerformanceCycle> GetAll()
    {
        return context.PerformanceCycles
            .OrderByDescending(x => x.StartDate)
            .ToList();
    }

    public PerformanceCycle? GetById(Guid id)
    {
        return context.PerformanceCycles.FirstOrDefault(x => x.Id == id);
    }

    public void Add(PerformanceCycle cycle)
    {
        context.PerformanceCycles.Add(cycle);
    }

    public void Update(PerformanceCycle cycle)
    {
        context.PerformanceCycles.Update(cycle);
    }

    public void Delete(PerformanceCycle cycle)
    {
        context.PerformanceCycles.Remove(cycle);
    }

    public bool HasOverlappingCycle(DateOnly startDate, DateOnly endDate, Guid? excludeId = null)
    {
        return context.PerformanceCycles.Any(x =>
            (!excludeId.HasValue || x.Id != excludeId.Value) &&
            startDate <= x.EndDate &&
            endDate >= x.StartDate);
    }

    public bool CycleNameExists(string name, Guid? excludeId = null)
    {
        return context.PerformanceCycles.Any(x =>
            x.Name.ToLower() == name.ToLower() &&
            (!excludeId.HasValue || x.Id != excludeId.Value));
    }

    public bool HasReviews(Guid cycleId)
    {
        return context.PerformanceReviews.Any(x => x.PerformanceCycleId == cycleId);
    }

    public bool HasRecommendations(Guid cycleId)
    {
        return context.PerformanceBonusRecommendations.Any(x => x.PerformanceCycleId == cycleId);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}
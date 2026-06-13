using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IPerformanceCycleRepository
{
    List<PerformanceCycle> GetAll();

    PerformanceCycle? GetById(
        Guid id);

    void Add(
        PerformanceCycle cycle);

    void Update(
        PerformanceCycle cycle);

    void Delete(
        PerformanceCycle cycle);

    bool HasOverlappingCycle(
        DateOnly startDate,
        DateOnly endDate,
        Guid? excludeId = null);

    void SaveChanges();



    bool CycleNameExists(
    string name,
    Guid? excludeId = null);

    bool HasReviews(
        Guid cycleId);

    bool HasRecommendations(
        Guid cycleId);
}
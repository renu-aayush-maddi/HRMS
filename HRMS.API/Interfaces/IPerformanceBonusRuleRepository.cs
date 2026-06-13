using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IPerformanceBonusRuleRepository
{
    List<PerformanceBonusRule> GetAll();

    PerformanceBonusRule? GetById(
        Guid id);

    void Add(
        PerformanceBonusRule rule);

    void Update(
        PerformanceBonusRule rule);

    void Delete(
        PerformanceBonusRule rule);

    void SaveChanges();
}
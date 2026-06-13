using HRMS.API.Models.DTOs.PerformanceBonusRule;

namespace HRMS.API.Interfaces;

public interface IPerformanceBonusRuleService
{
    void AddRule(
        AddPerformanceBonusRuleDto dto);

    void UpdateRule(
        Guid id,
        UpdatePerformanceBonusRuleDto dto);

    void DeleteRule(
        Guid id);

    List<PerformanceBonusRuleResponseDto>
        GetAllRules();

    PerformanceBonusRuleResponseDto
        GetRuleById(
            Guid id);
}
using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.PerformanceBonusRule;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

public class PerformanceBonusRuleService
    : IPerformanceBonusRuleService
{
    private readonly
        IPerformanceBonusRuleRepository
        repository;

    public PerformanceBonusRuleService(
        IPerformanceBonusRuleRepository repository)
    {
        this.repository = repository;
    }

    public void AddRule(
        AddPerformanceBonusRuleDto dto)
    {
        if(dto.MinRating > dto.MaxRating)
        {
            throw new BusinessException(
                "MinRating cannot be greater than MaxRating");
        }

        var rule =
            new PerformanceBonusRule
            {
                Id = Guid.NewGuid(),

                MinRating =
                    dto.MinRating,

                MaxRating =
                    dto.MaxRating,

                BonusPercentage =
                    dto.BonusPercentage,

                IsActive = true,

                CreatedAt =
                    DateTime.UtcNow
            };

        repository.Add(rule);

        repository.SaveChanges();
    }

    public void UpdateRule(
        Guid id,
        UpdatePerformanceBonusRuleDto dto)
    {
        var rule =
            repository.GetById(id);

        if(rule == null)
        {
            throw new NotFoundException(
                "Rule not found");
        }

        rule.MinRating =
            dto.MinRating;

        rule.MaxRating =
            dto.MaxRating;

        rule.BonusPercentage =
            dto.BonusPercentage;

        rule.IsActive =
            dto.IsActive;

        repository.Update(rule);

        repository.SaveChanges();
    }

    public void DeleteRule(
        Guid id)
    {
        var rule =
            repository.GetById(id);

        if(rule == null)
        {
            throw new NotFoundException(
                "Rule not found");
        }

        repository.Delete(rule);

        repository.SaveChanges();
    }

    public List<PerformanceBonusRuleResponseDto>
        GetAllRules()
    {
        return repository
            .GetAll()
            .Select(r =>
                new PerformanceBonusRuleResponseDto
                {
                    Id = r.Id,

                    MinRating =
                        r.MinRating,

                    MaxRating =
                        r.MaxRating,

                    BonusPercentage =
                        r.BonusPercentage,

                    IsActive =
                        r.IsActive ?? false
                })
            .ToList();
    }

    public PerformanceBonusRuleResponseDto
        GetRuleById(Guid id)
    {
        var rule =
            repository.GetById(id);

        if(rule == null)
        {
            throw new NotFoundException(
                "Rule not found");
        }

        return new PerformanceBonusRuleResponseDto
        {
            Id = rule.Id,

            MinRating =
                rule.MinRating,

            MaxRating =
                rule.MaxRating,

            BonusPercentage =
                rule.BonusPercentage,

            IsActive =
                rule.IsActive ?? false
        };
    }
}
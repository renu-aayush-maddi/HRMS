namespace HRMS.API.Models.DTOs.PerformanceBonusRule;

public class PerformanceBonusRuleResponseDto
{
    public Guid Id { get; set; }

    public decimal MinRating { get; set; }

    public decimal MaxRating { get; set; }

    public decimal BonusPercentage { get; set; }

    public bool IsActive { get; set; }
}
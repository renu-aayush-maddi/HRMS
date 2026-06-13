using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models.DTOs.PerformanceBonusRule;

public class AddPerformanceBonusRuleDto
{
    [Required]
    public decimal MinRating { get; set; }

    [Required]
    public decimal MaxRating { get; set; }

    [Required]
    public decimal BonusPercentage { get; set; }
}
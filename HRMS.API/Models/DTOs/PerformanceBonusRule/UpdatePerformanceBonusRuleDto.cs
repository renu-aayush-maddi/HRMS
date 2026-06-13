using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models.DTOs.PerformanceBonusRule;

public class UpdatePerformanceBonusRuleDto
{
    [Required]
    public decimal MinRating { get; set; }

    [Required]
    public decimal MaxRating { get; set; }

    [Required]
    public decimal BonusPercentage { get; set; }

    [Required]
    public bool IsActive { get; set; }
}   
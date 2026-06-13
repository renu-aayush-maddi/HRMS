using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models.DTOs.PerformanceBonusRecommendation;

public class UpdatePerformanceBonusRecommendationDto
{
    [Required]
    public decimal ApprovedAmount { get; set; }

    public string? Remarks { get; set; }
}
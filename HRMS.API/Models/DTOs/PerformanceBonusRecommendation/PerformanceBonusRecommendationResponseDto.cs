namespace HRMS.API.Models.DTOs.PerformanceBonusRecommendation;

public class PerformanceBonusRecommendationResponseDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string EmployeeName { get; set; }
        = string.Empty;

    public decimal AverageRating { get; set; }

    public decimal RecommendedPercentage { get; set; }

    public decimal RecommendedAmount { get; set; }

    public decimal? ApprovedAmount { get; set; }

    public string Status { get; set; }
        = string.Empty;

    public string? Remarks { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models.DTOs.Review;

public class AddReviewDto
{
    public Guid EmployeeId { get; set; }

    public Guid PerformanceCycleId { get; set; }

    [Range(1,5)]
    public decimal Rating { get; set; }

    public string? Comments { get; set; }
}
namespace HRMS.API.Models.DTOs.Review;

public class ReviewResponseDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid ReviewerId { get; set; }

    public Guid PerformanceCycleId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public string ReviewerName { get; set; } = string.Empty;

    public string PerformanceCycleName { get; set; } = string.Empty;

    public decimal? Rating { get; set; }

    public string? Comments { get; set; }

    public DateOnly? ReviewDate { get; set; }

    public DateTime? CreatedAt { get; set; }
}
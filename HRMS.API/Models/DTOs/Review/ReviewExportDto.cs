namespace HRMS.API.Models.DTOs.Review;

public class ReviewExportDto
{
    public string EmployeeName { get; set; } = string.Empty;

    public string ReviewerName { get; set; } = string.Empty;

    public string PerformanceCycle { get; set; } = string.Empty;

    public decimal? Rating { get; set; }

    public string? Comments { get; set; }

    public DateOnly? ReviewDate { get; set; }
}
namespace HRMS.API.Models.DTOs.Manager;

public class PerformanceReviewDto
{
    public Guid Id { get; set; }

    public string EmployeeName { get; set; }
        = string.Empty;

    public decimal? Rating { get; set; }

    public string? Comments { get; set; }

    public DateOnly? ReviewDate { get; set; }
}
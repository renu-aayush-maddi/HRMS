namespace HRMS.API.Models.DTOs.Review;

public class ReviewResponseDto
{
    public Guid Id { get; set; }

    public string EmployeeName { get; set; }

    public string ReviewerName { get; set; }

    public decimal? Rating { get; set; }

    public string? Comments { get; set; }

    public DateOnly? ReviewDate { get; set; }
}
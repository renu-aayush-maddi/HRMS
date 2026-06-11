namespace HRMS.API.Models.DTOs.Review;

public class AddReviewDto
{
    public Guid EmployeeId { get; set; }

    public Guid ReviewerId { get; set; }

    public decimal Rating { get; set; }

    public string? Comments { get; set; }
}
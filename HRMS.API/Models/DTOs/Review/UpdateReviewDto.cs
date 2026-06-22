namespace HRMS.API.Models.DTOs.Review;

public class UpdateReviewDto
{
    public decimal Rating { get; set; }

    public string? Comments { get; set; }
}
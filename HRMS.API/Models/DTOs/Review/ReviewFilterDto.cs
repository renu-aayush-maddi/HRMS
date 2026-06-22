using HRMS.API.Models.Common;

namespace HRMS.API.Models.DTOs.Review;

public class ReviewFilterDto : PaginationRequestDto
{
    public Guid? EmployeeId { get; set; }

    public Guid? ReviewerId { get; set; }

    public Guid? PerformanceCycleId { get; set; }

    public decimal? MinRating { get; set; }

    public decimal? MaxRating { get; set; }

    public DateOnly? FromReviewDate { get; set; }

    public DateOnly? ToReviewDate { get; set; }

    public string? SortBy { get; set; }

    public bool Descending { get; set; }
}
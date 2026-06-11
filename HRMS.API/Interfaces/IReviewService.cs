using HRMS.API.Models.DTOs.Review;

namespace HRMS.API.Interfaces;

public interface IReviewService
{
    void AddReview(AddReviewDto dto);

    List<ReviewResponseDto> GetAllReviews();

    List<ReviewResponseDto>
        GetEmployeeReviews(Guid employeeId);
}
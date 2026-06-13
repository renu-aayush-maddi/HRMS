using HRMS.API.Models.DTOs.Review;

namespace HRMS.API.Interfaces;

public interface IReviewService
{
    void AddReview(Guid reviewerUserId,AddReviewDto dto);

    List<ReviewResponseDto> GetAllReviews();

    List<ReviewResponseDto> GetEmployeeReviews(Guid employeeId);

    List<ReviewResponseDto> GetMyReviews(Guid employeeUserId);

}
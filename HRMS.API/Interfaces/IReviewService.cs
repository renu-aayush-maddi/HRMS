using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.Review;

namespace HRMS.API.Interfaces;

public interface IReviewService
{
    Task<ReviewResponseDto> AddReviewAsync(AddReviewDto dto, CancellationToken cancellationToken = default);

    Task<PagedResponse<ReviewResponseDto>> GetReviewsAsync(ReviewFilterDto filter, CancellationToken cancellationToken = default);

    Task<ReviewResponseDto> GetReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);

    Task<PagedResponse<ReviewResponseDto>> GetEmployeeReviewsAsync(Guid employeeId, ReviewFilterDto filter, CancellationToken cancellationToken = default);

    Task<PagedResponse<ReviewResponseDto>> GetMyReviewsAsync(ReviewFilterDto filter, CancellationToken cancellationToken = default);

    Task<ReviewResponseDto> UpdateReviewAsync(Guid reviewId, UpdateReviewDto dto, CancellationToken cancellationToken = default);

    Task DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);

    Task<byte[]> ExportReviewsAsync(ReviewFilterDto filter, CancellationToken cancellationToken = default);
}
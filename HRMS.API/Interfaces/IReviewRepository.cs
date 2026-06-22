using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IReviewRepository
{
    Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Employee?> GetReviewerAsync(Guid reviewerId, CancellationToken cancellationToken = default);

    Task<PerformanceCycle?> GetCycleAsync(Guid cycleId, CancellationToken cancellationToken = default);

    Task<PerformanceReview?> GetReviewAsync(Guid reviewId, CancellationToken cancellationToken = default);

    IQueryable<PerformanceReview> GetReviews();

    Task<bool> ReviewExistsAsync(Guid employeeId, Guid reviewerId, Guid performanceCycleId, CancellationToken cancellationToken = default);

    Task<bool> ReviewExistsAsync(Guid employeeId, Guid reviewerId, Guid performanceCycleId, Guid reviewId, CancellationToken cancellationToken = default);

    Task AddReviewAsync(PerformanceReview review, CancellationToken cancellationToken = default);

    void UpdateReview(PerformanceReview review);

    void DeleteReview(PerformanceReview review);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
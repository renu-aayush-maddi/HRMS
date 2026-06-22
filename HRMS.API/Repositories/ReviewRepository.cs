using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext context;

    public ReviewRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted, cancellationToken);
    }

    public async Task<Employee?> GetEmployeeByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(e => e.UserId == userId && !e.IsDeleted, cancellationToken);
    }

    public async Task<PerformanceCycle?> GetCycleAsync(Guid cycleId, CancellationToken cancellationToken = default)
    {
        return await context.PerformanceCycles
            .FirstOrDefaultAsync(x => x.Id == cycleId, cancellationToken);
    }

    public async Task<PerformanceReview?> GetReviewAsync(Guid reviewId, CancellationToken cancellationToken = default)
    {
        return await context.PerformanceReviews
            .Include(x => x.Employee)
            .Include(x => x.Reviewer)
            .Include(x => x.PerformanceCycle)
            .FirstOrDefaultAsync(x => x.Id == reviewId, cancellationToken);
    }

    public IQueryable<PerformanceReview> GetReviews()
    {
        return context.PerformanceReviews
            .AsNoTracking()
            .Include(x => x.Employee)
            .Include(x => x.Reviewer)
            .Include(x => x.PerformanceCycle);
    }

    public async Task<bool> ReviewExistsAsync(
        Guid employeeId,
        Guid reviewerId,
        Guid performanceCycleId,
        Guid reviewId,
        CancellationToken cancellationToken = default)
    {
        return await context.PerformanceReviews
            .AnyAsync(
                x =>
                    x.EmployeeId == employeeId &&
                    x.ReviewerId == reviewerId &&
                    x.PerformanceCycleId == performanceCycleId &&
                    x.Id != reviewId,
                cancellationToken);
    }


    public async Task<bool> ReviewExistsAsync(
    Guid employeeId,
    Guid reviewerId,
    Guid performanceCycleId,
    CancellationToken cancellationToken = default)
    {
        return await context.PerformanceReviews
            .AnyAsync(
                x =>
                    x.EmployeeId == employeeId &&
                    x.ReviewerId == reviewerId &&
                    x.PerformanceCycleId == performanceCycleId,
                cancellationToken);
    }

    public async Task<Employee?> GetReviewerAsync(
    Guid reviewerId,
    CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(
                x => x.Id == reviewerId &&
                     !x.IsDeleted,
                cancellationToken);
    }

    public async Task AddReviewAsync(PerformanceReview review, CancellationToken cancellationToken = default)
    {
        await context.PerformanceReviews.AddAsync(review, cancellationToken);
    }

    public void UpdateReview(PerformanceReview review)
    {
        context.PerformanceReviews.Update(review);
    }

    public void DeleteReview(PerformanceReview review)
    {
        context.PerformanceReviews.Remove(review);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
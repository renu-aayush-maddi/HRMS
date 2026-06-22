using ClosedXML.Excel;
using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.Review;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository repository;
    private readonly IUserContextService userContextService;
    private readonly IAuditLogService auditLogService;
    private readonly INotificationService notificationService;
    private readonly ILogger<ReviewService> logger;

    public ReviewService(
        IReviewRepository repository,
        IUserContextService userContextService,
        IAuditLogService auditLogService,
        INotificationService notificationService,
        ILogger<ReviewService> logger)
    {
        this.repository = repository;
        this.userContextService = userContextService;
        this.auditLogService = auditLogService;
        this.notificationService = notificationService;
        this.logger = logger;
    }

    public async Task<ReviewResponseDto> AddReviewAsync(
        AddReviewDto dto,
        CancellationToken cancellationToken = default)
    {
        var reviewerUserId = userContextService.GetUserId();

        var reviewer = await repository.GetEmployeeByUserIdAsync(reviewerUserId, cancellationToken);
        if (reviewer == null)
        {
            throw new NotFoundException("Reviewer not found.");
        }

        var employee = await repository.GetEmployeeAsync(dto.EmployeeId, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException("Employee not found.");
        }

        if (employee.ManagerId != reviewer.Id)
        {
            throw new ForbiddenException("Employee does not belong to your team.");
        }

        var cycle = await repository.GetCycleAsync(dto.PerformanceCycleId, cancellationToken);
        if (cycle == null)
        {
            throw new NotFoundException("Performance cycle not found.");
        }

        if (!string.Equals(cycle.Status, "Active", StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessException("Performance cycle is not active.");
        }

        var today = DateOnly.FromDateTime(DateTime.Now);
        if (today < cycle.StartDate || today > cycle.EndDate)
        {
            throw new BusinessException("Reviews can only be submitted during the performance cycle.");
        }

        var reviewExists = await repository.ReviewExistsAsync(employee.Id, reviewer.Id, cycle.Id, cancellationToken);
        if (reviewExists)
        {
            throw new BusinessException("Review already exists for this employee in the selected cycle.");
        }

        var review = new PerformanceReview
        {
            Id = Guid.NewGuid(),
            EmployeeId = employee.Id,
            ReviewerId = reviewer.Id,
            PerformanceCycleId = cycle.Id,
            Rating = dto.Rating,
            Comments = dto.Comments,
            ReviewDate = today,
            CreatedAt = DateTime.Now
        };

        await repository.AddReviewAsync(review, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Create",
            nameof(PerformanceReview),
            review.Id,
            $"Performance review created for employee {employee.Id}",
            cancellationToken);

        logger.LogInformation("Review {ReviewId} created by manager {ReviewerId}", review.Id, reviewer.Id);

        if (employee.UserId.HasValue)
        {
            notificationService.CreateNotification(
                employee.UserId.Value,
                "Performance Review Added",
                "A new performance review has been added for you.");
        }

        return await GetReviewAsync(review.Id, cancellationToken);
    }

    public async Task<ReviewResponseDto> GetReviewAsync(
        Guid reviewId,
        CancellationToken cancellationToken = default)
    {
        var review = await repository.GetReviewAsync(reviewId, cancellationToken);
        if (review == null)
        {
            throw new NotFoundException("Review not found.");
        }

        return MapToResponse(review);
    }

    public async Task<PagedResponse<ReviewResponseDto>> GetReviewsAsync(
        ReviewFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var role = userContextService.GetRole();
        var query = repository.GetReviews();

        if (!userContextService.IsAdminOrHr())
        {
            if (role == "Manager")
            {
                var manager = await repository.GetEmployeeByUserIdAsync(userContextService.GetUserId(), cancellationToken);
                if (manager == null)
                {
                    throw new NotFoundException("Manager profile not found.");
                }

                query = query.Where(x => x.ReviewerId == manager.Id);
            }
            else
            {
                var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
                if (employeeId == null)
                {
                    throw new NotFoundException("Employee profile not found.");
                }

                query = query.Where(x => x.EmployeeId == employeeId);
            }
        }

        query = ApplyFilters(query, filter);
        query = ApplySorting(query, filter);

        var totalRecords = await query.CountAsync(cancellationToken);

        var reviews = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<ReviewResponseDto>
        {
            Data = reviews.Select(MapToResponse),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize)
        };
    }

    public async Task<PagedResponse<ReviewResponseDto>> GetEmployeeReviewsAsync(
        Guid employeeId,
        ReviewFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        if (!userContextService.IsAdminOrHr())
        {
            throw new ForbiddenException("Only Admin and HR can access employee reviews.");
        }

        filter.EmployeeId = employeeId;
        return await GetReviewsAsync(filter, cancellationToken);
    }

    public async Task<PagedResponse<ReviewResponseDto>> GetMyReviewsAsync(
        ReviewFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
        if (employeeId == null)
        {
            throw new NotFoundException("Employee profile not found.");
        }

        filter.EmployeeId = employeeId;
        return await GetReviewsAsync(filter, cancellationToken);
    }

    public async Task<ReviewResponseDto> UpdateReviewAsync(
        Guid reviewId,
        UpdateReviewDto dto,
        CancellationToken cancellationToken = default)
    {
        var review = await repository.GetReviewAsync(reviewId, cancellationToken);
        if (review == null)
        {
            throw new NotFoundException("Review not found.");
        }

        var manager = await repository.GetEmployeeByUserIdAsync(userContextService.GetUserId(), cancellationToken);
        if (manager == null)
        {
            throw new NotFoundException("Manager profile not found.");
        }

        if (review.ReviewerId != manager.Id)
        {
            throw new ForbiddenException("You can only update reviews created by you.");
        }

        var cycle = await repository.GetCycleAsync(review.PerformanceCycleId, cancellationToken);
        if (cycle == null)
        {
            throw new NotFoundException("Performance cycle not found.");
        }

        if (!string.Equals(cycle.Status, "Active", StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessException("Cannot update review after cycle is closed.");
        }

        review.Rating = dto.Rating;
        review.Comments = dto.Comments;

        repository.UpdateReview(review);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Update",
            nameof(PerformanceReview),
            review.Id,
            "Performance review updated.",
            cancellationToken);

        logger.LogInformation("Review {ReviewId} updated", review.Id);

        return MapToResponse(review);
    }

    public async Task DeleteReviewAsync(
        Guid reviewId,
        CancellationToken cancellationToken = default)
    {
        if (!userContextService.IsAdminOrHr())
        {
            throw new ForbiddenException("Only Admin and HR can delete reviews.");
        }

        var review = await repository.GetReviewAsync(reviewId, cancellationToken);
        if (review == null)
        {
            throw new NotFoundException("Review not found.");
        }

        repository.DeleteReview(review);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Delete",
            nameof(PerformanceReview),
            review.Id,
            "Performance review deleted.",
            cancellationToken);

        logger.LogInformation("Review {ReviewId} deleted", review.Id);
    }

    public async Task<byte[]> ExportReviewsAsync(
        ReviewFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        if (!userContextService.IsAdminOrHr())
        {
            throw new ForbiddenException("Only Admin and HR can export reviews.");
        }

        var query = repository.GetReviews();
        query = ApplyFilters(query, filter);

        var reviews = await query
            .OrderByDescending(x => x.ReviewDate)
            .ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Reviews");

        worksheet.Cell(1, 1).Value = "Employee";
        worksheet.Cell(1, 2).Value = "Reviewer";
        worksheet.Cell(1, 3).Value = "Cycle";
        worksheet.Cell(1, 4).Value = "Rating";
        worksheet.Cell(1, 5).Value = "Review Date";
        worksheet.Cell(1, 6).Value = "Comments";

        var row = 2;
        foreach (var review in reviews)
        {
            worksheet.Cell(row, 1).Value = $"{review.Employee?.FirstName} {review.Employee?.LastName}";
            worksheet.Cell(row, 2).Value = $"{review.Reviewer?.FirstName} {review.Reviewer?.LastName}";
            worksheet.Cell(row, 3).Value = review.PerformanceCycle.Name;
            worksheet.Cell(row, 4).Value = review.Rating;
            worksheet.Cell(row, 5).Value = review.ReviewDate?.ToString();
            worksheet.Cell(row, 6).Value = review.Comments;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        await auditLogService.LogAsync(
            "Export",
            nameof(PerformanceReview),
            Guid.Empty,
            $"{reviews.Count} reviews exported.",
            cancellationToken);

        return stream.ToArray();
    }

    private static IQueryable<PerformanceReview> ApplyFilters(
        IQueryable<PerformanceReview> query,
        ReviewFilterDto filter)
    {
        if (filter.EmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == filter.EmployeeId);
        }

        if (filter.ReviewerId.HasValue)
        {
            query = query.Where(x => x.ReviewerId == filter.ReviewerId);
        }

        if (filter.PerformanceCycleId.HasValue)
        {
            query = query.Where(x => x.PerformanceCycleId == filter.PerformanceCycleId);
        }

        if (filter.MinRating.HasValue)
        {
            query = query.Where(x => x.Rating >= filter.MinRating);
        }

        if (filter.MaxRating.HasValue)
        {
            query = query.Where(x => x.Rating <= filter.MaxRating);
        }

        if (filter.FromReviewDate.HasValue)
        {
            query = query.Where(x => x.ReviewDate >= filter.FromReviewDate);
        }

        if (filter.ToReviewDate.HasValue)
        {
            query = query.Where(x => x.ReviewDate <= filter.ToReviewDate);
        }

        return query;
    }

    private static IQueryable<PerformanceReview> ApplySorting(
        IQueryable<PerformanceReview> query,
        ReviewFilterDto filter)
    {
        return filter.SortBy?.ToLower() switch
        {
            "rating" => filter.Descending 
                ? query.OrderByDescending(x => x.Rating) 
                : query.OrderBy(x => x.Rating),

            "reviewdate" => filter.Descending 
                ? query.OrderByDescending(x => x.ReviewDate) 
                : query.OrderBy(x => x.ReviewDate),

            _ => query.OrderByDescending(x => x.CreatedAt)
        };
    }

    private static ReviewResponseDto MapToResponse(PerformanceReview review)
    {
        return new ReviewResponseDto
        {
            Id = review.Id,
            EmployeeId = review.EmployeeId ?? Guid.Empty,
            ReviewerId = review.ReviewerId ?? Guid.Empty,
            PerformanceCycleId = review.PerformanceCycleId,
            EmployeeName = $"{review.Employee?.FirstName} {review.Employee?.LastName}",
            ReviewerName = $"{review.Reviewer?.FirstName} {review.Reviewer?.LastName}",
            PerformanceCycleName = review.PerformanceCycle?.Name ?? string.Empty,
            Rating = review.Rating,
            Comments = review.Comments,
            ReviewDate = review.ReviewDate,
            CreatedAt = review.CreatedAt
        };
    }
}
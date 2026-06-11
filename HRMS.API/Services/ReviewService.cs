using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Review;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository reviewRepository;
    private readonly INotificationService notificationService;
    public ReviewService(IReviewRepository reviewRepository, INotificationService notificationService)
    {
        this.reviewRepository = reviewRepository;
        this.notificationService = notificationService;
    }

    public void AddReview(AddReviewDto dto)
    {
        var employee =
            reviewRepository.GetEmployee(dto.EmployeeId);

        if (employee == null)
        {
            throw new Exception("Employee not found");
        }

        var reviewer =
            reviewRepository.GetEmployee(dto.ReviewerId);

        if (reviewer == null)
        {
            throw new Exception("Reviewer not found");
        }

        PerformanceReview review =
            new PerformanceReview
            {
                Id = Guid.NewGuid(),

                EmployeeId = dto.EmployeeId,

                ReviewerId = dto.ReviewerId,

                Rating = dto.Rating,

                Comments = dto.Comments,

                ReviewDate =
                    DateOnly.FromDateTime(DateTime.Now)
            };

        reviewRepository.AddReview(review);

        reviewRepository.SaveChanges();

        if (employee.UserId != null)
        {
            notificationService.CreateNotification(
                employee.UserId.Value,
                "Performance Review Added",
                "A new performance review has been added for you.");
        }
    }

    public List<ReviewResponseDto> GetAllReviews()
    {
        return reviewRepository
            .GetAllReviews()
            .Select(r => new ReviewResponseDto
            {
                Id = r.Id,

                EmployeeName =
                    r.Employee!.FirstName + " " +
                    r.Employee.LastName,

                ReviewerName =
                    r.Reviewer!.FirstName + " " +
                    r.Reviewer.LastName,

                Rating = r.Rating,

                Comments = r.Comments,

                ReviewDate = r.ReviewDate
            })
            .ToList();
    }

    public List<ReviewResponseDto>
        GetEmployeeReviews(Guid employeeId)
    {
        return reviewRepository
            .GetEmployeeReviews(employeeId)
            .Select(r => new ReviewResponseDto
            {
                Id = r.Id,

                EmployeeName =
                    r.Employee!.FirstName + " " +
                    r.Employee.LastName,

                ReviewerName =
                    r.Reviewer!.FirstName + " " +
                    r.Reviewer.LastName,

                Rating = r.Rating,

                Comments = r.Comments,

                ReviewDate = r.ReviewDate
            })
            .ToList();
    }
}
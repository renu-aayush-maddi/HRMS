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

    public void AddReview(Guid reviewerUserId,AddReviewDto dto)
    {

        var cycle =reviewRepository.GetCycle(dto.PerformanceCycleId);

        if(cycle == null)
        {
            throw new Exception("Performance cycle not found");
        }
        var employee = reviewRepository.GetEmployee(dto.EmployeeId);

        if (employee == null)
        {
            throw new Exception("Employee not found");
        }

        var reviewer =reviewRepository.GetEmployeeByUserId(reviewerUserId);

        if (reviewer == null)
        {
            throw new Exception("Reviewer not found");
        }

        if(employee.ManagerId != reviewer.Id)
        {
            throw new Exception("Employee does not belong to your team");
        }

        if(dto.Rating < 1 || dto.Rating > 5)
        {
            throw new Exception("Rating must be between 1 and 5");
        }


        PerformanceReview review =
            new PerformanceReview
            {
                Id = Guid.NewGuid(),

                EmployeeId = dto.EmployeeId,

                ReviewerId = reviewer.Id,

                Rating = dto.Rating,

                Comments = dto.Comments,

                ReviewDate = DateOnly.FromDateTime(DateTime.Now),

                PerformanceCycleId = dto.PerformanceCycleId,
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


    public List<ReviewResponseDto>
    GetMyReviews(
        Guid employeeUserId)
{
    var employee =
        reviewRepository
        .GetEmployeeByUserId(
            employeeUserId);

    if (employee == null)
    {
        throw new Exception(
            "Employee not found");
    }

    return reviewRepository
        .GetEmployeeReviews(
            employee.Id)
        .Select(r =>
            new ReviewResponseDto
            {
                Id = r.Id,

                EmployeeName =
                    r.Employee!.FirstName +
                    " " +
                    r.Employee.LastName,

                ReviewerName =
                    r.Reviewer!.FirstName +
                    " " +
                    r.Reviewer.LastName,

                Rating = r.Rating,

                Comments = r.Comments,

                ReviewDate =
                    r.ReviewDate
            })
        .ToList();
}
}
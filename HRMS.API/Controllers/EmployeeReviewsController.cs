using HRMS.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.API.Controllers;

[Route("api/employee/reviews")]
[ApiController]
[Authorize(Roles = "Employee")]
public class EmployeeReviewsController : ControllerBase
{
    private readonly IReviewService reviewService;

    public EmployeeReviewsController(
        IReviewService reviewService)
    {
        this.reviewService = reviewService;
    }

    [HttpGet]
    public IActionResult GetMyReviews()
    {
        var userId =
            Guid.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!
                .Value);

        return Ok(
            reviewService
            .GetMyReviews(userId));
    }
}
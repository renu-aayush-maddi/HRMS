using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        this.reviewService = reviewService;
    }

    [Authorize(Roles = "Manager")]
    [HttpPost]
    public IActionResult AddReview(AddReviewDto dto)
    {
        var reviewerUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        reviewService.AddReview(reviewerUserId,dto);

        return Ok("Review Added Successfully");
    }


    [Authorize(Roles = "Admin,HR,Manager")]
    [HttpGet]
    public IActionResult GetAllReviews()
    {
        return Ok(reviewService.GetAllReviews());
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("employee/{employeeId}")]
    public IActionResult GetEmployeeReviews( Guid employeeId)
    {
        return Ok(reviewService.GetEmployeeReviews(employeeId));
    }
}
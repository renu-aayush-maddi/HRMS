using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService reviewService;

    public ReviewsController(
        IReviewService reviewService)
    {
        this.reviewService = reviewService;
    }

    [Authorize(Roles = "Admin,HR,Manager")]
    [HttpPost]
    public IActionResult AddReview(
        AddReviewDto dto)
    {
        reviewService.AddReview(dto);

        return Ok("Review Added Successfully");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public IActionResult GetAllReviews()
    {
        return Ok(
            reviewService.GetAllReviews());
    }

    [Authorize]
    [HttpGet("employee/{employeeId}")]
    public IActionResult GetEmployeeReviews(
        Guid employeeId)
    {
        return Ok(
            reviewService
            .GetEmployeeReviews(employeeId));
    }
}
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Review;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/reviews")]
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
    public async Task<IActionResult> AddReview([FromBody] AddReviewDto dto, CancellationToken cancellationToken)
    {
        var result = await reviewService.AddReviewAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetReview), new { reviewId = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetReviews([FromQuery] ReviewFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await reviewService.GetReviewsAsync(filter, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{reviewId:guid}")]
    public async Task<IActionResult> GetReview(Guid reviewId, CancellationToken cancellationToken)
    {
        var result = await reviewService.GetReviewAsync(reviewId, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("employee/{employeeId:guid}")]
    public async Task<IActionResult> GetEmployeeReviews(Guid employeeId, [FromQuery] ReviewFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await reviewService.GetEmployeeReviewsAsync(employeeId, filter, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Employee")]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyReviews([FromQuery] ReviewFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await reviewService.GetMyReviewsAsync(filter, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Manager")]
    [HttpPut("{reviewId:guid}")]
    public async Task<IActionResult> UpdateReview(Guid reviewId, [FromBody] UpdateReviewDto dto, CancellationToken cancellationToken)
    {
        var result = await reviewService.UpdateReviewAsync(reviewId, dto, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpDelete("{reviewId:guid}")]
    public async Task<IActionResult> DeleteReview(Guid reviewId, CancellationToken cancellationToken)
    {
        await reviewService.DeleteReviewAsync(reviewId, cancellationToken);
        return Ok(new { Message = "Review deleted successfully." });
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("export")]
    public async Task<IActionResult> ExportReviews([FromQuery] ReviewFilterDto filter, CancellationToken cancellationToken)
    {
        var fileBytes = await reviewService.ExportReviewsAsync(filter, cancellationToken);
        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"performance-reviews-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }
}
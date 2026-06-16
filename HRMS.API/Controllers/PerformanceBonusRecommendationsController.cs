using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.PerformanceBonusRecommendation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/performance-bonus-recommendations")]
[ApiController]
[Authorize(Roles = "Admin,HR")]
public class PerformanceBonusRecommendationsController : ControllerBase
{
    private readonly IPerformanceBonusRecommendationService service;

    public PerformanceBonusRecommendationsController(IPerformanceBonusRecommendationService service)
    {
        this.service = service;
    }

    [HttpPost("generate/{cycleId}")]
    public IActionResult Generate(Guid cycleId)
    {
        service.GenerateRecommendations(cycleId);

        return Ok("Recommendations Generated");
    }

    [HttpGet]
    public IActionResult GetRecommendations()
    {
        return Ok(service.GetRecommendations());
    }

    [HttpPut("{id}")]
    public IActionResult UpdateRecommendation(Guid id, UpdatePerformanceBonusRecommendationDto dto)
    {
        service.UpdateRecommendation(id, dto);

        return Ok("Recommendation Updated");
    }

    [HttpPost("{id}/approve")]
    public IActionResult Approve(Guid id)
    {
        service.ApproveRecommendation(id);

        return Ok("Recommendation Approved");
    }

    [HttpPost("{id}/reject")]
    public IActionResult Reject(Guid id)
    {
        service.RejectRecommendation(id);

        return Ok("Recommendation Rejected");
    }

    [HttpGet("{id}")]
    public IActionResult GetRecommendation(Guid id)
    {
        return Ok(service.GetRecommendation(id));
    }
}
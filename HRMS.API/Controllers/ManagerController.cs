using HRMS.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HRMS.API.Models.DTOs.Manager;

namespace HRMS.API.Controllers;

[Route("api/manager")]
[ApiController]
[Authorize(Roles = "Manager")]
public class ManagerController : ControllerBase
{
    private readonly IManagerService service;

    public ManagerController(IManagerService service)
    {
        this.service = service;
    }

    [HttpGet("dashboard")]
    public IActionResult GetDashboard()
    {
        var managerUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return Ok(service.GetDashboard(managerUserId));
    }

    [HttpGet("team")]
    public IActionResult GetTeamMembers()
    {
        var managerUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return Ok(service.GetTeamMembers(managerUserId));
    }

    [HttpGet("team/{employeeId}")]
    public IActionResult GetTeamMember(Guid employeeId)
    {
        var managerUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return Ok(service.GetTeamMember(managerUserId, employeeId));
    }

    [HttpGet("team-attendance")]
    public IActionResult GetTeamAttendance()
    {
        var managerUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return Ok(service.GetTeamAttendance(managerUserId));
    }

    [HttpGet("late-employees")]
    public IActionResult GetLateEmployees()
    {
        var managerUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return Ok(service.GetLateEmployees(managerUserId));
    }

    [HttpGet("pending-leaves")]
    public IActionResult GetPendingLeaveRequests()
    {
        var managerUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return Ok(service.GetPendingLeaveRequests(managerUserId));
    }

    [HttpPost("performance-reviews")]
    public IActionResult AddPerformanceReview(AddPerformanceReviewDto dto)
    {
        var managerUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        service.AddPerformanceReview(managerUserId, dto);

        return Ok("Performance Review Added");
    }

    [HttpGet("performance-reviews")]
    public IActionResult GetPerformanceReviews()
    {
        var managerUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return Ok(service.GetPerformanceReviews(managerUserId));
    }

    [HttpGet("performance-reviews/{employeeId}")]
    public IActionResult GetEmployeePerformanceReviews(Guid employeeId)
    {
        var managerUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return Ok(service.GetEmployeePerformanceReviews(managerUserId, employeeId));
    }
}
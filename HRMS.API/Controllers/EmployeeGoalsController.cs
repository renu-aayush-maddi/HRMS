using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Goal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.API.Controllers;

[Route("api/employee/goals")]
[ApiController]
[Authorize(Roles = "Employee")]
public class EmployeeGoalsController : ControllerBase
{
    private readonly IGoalService service;

    public EmployeeGoalsController(
        IGoalService service)
    {
        this.service = service;
    }

    [HttpGet]
    public IActionResult GetMyGoals()
    {
        var userId =
            Guid.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!
                .Value);

        return Ok(
            service.GetMyGoals(userId));
    }

    [HttpPut("{goalId}")]
    public IActionResult UpdateMyGoalStatus(
        Guid goalId,
        UpdateGoalStatusDto dto)
    {
        var userId =
            Guid.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!
                .Value);

        service.UpdateMyGoalStatus(
            userId,
            goalId,
            dto);

        return Ok(
            "Goal Status Updated");
    }
}
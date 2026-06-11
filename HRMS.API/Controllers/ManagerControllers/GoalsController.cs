using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Goal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.API.Controllers;

[Route("api/manager/goals")]
[ApiController]
[Authorize(Roles = "Manager")]
public class GoalsController: ControllerBase
{
    private readonly IGoalService service;

    public GoalsController(IGoalService service)
    {
        this.service = service;
    }

    [HttpPost]
    public IActionResult AddGoal(AddGoalDto dto)
    {
        var managerUserId =Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        service.AddGoal(managerUserId,dto);

        return Ok("Goal Assigned Successfully");
    }

    [HttpGet]
    public IActionResult GetGoals([FromQuery] GoalQueryDto query)
    {
        var managerUserId =Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return Ok(service.GetGoals(managerUserId,query));
    }
    

    [HttpGet("{employeeId}")]
    public IActionResult GetEmployeeGoals(Guid employeeId)
    {
        var managerUserId =Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return Ok(service.GetEmployeeGoals(managerUserId,employeeId));
    }

    [HttpPut("{goalId}")]
    public IActionResult UpdateGoalStatus(Guid goalId,UpdateGoalStatusDto dto)
    {
        var managerUserId =Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        service.UpdateGoalStatus(managerUserId,goalId,dto);

        return Ok("Goal Status Updated");
    }
}
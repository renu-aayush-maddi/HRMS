using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Leave;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LeaveController : ControllerBase
{
    private readonly ILeaveService leaveService;

    public LeaveController(ILeaveService leaveService)
    {
        this.leaveService = leaveService;
    }

    [Authorize(Roles = "Employee,HR,Manager,Admin")]
    [HttpPost("apply")]
    public IActionResult ApplyLeave(ApplyLeaveDto dto)
    {

        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var role = User.FindFirst(ClaimTypes.Role)!.Value;

        leaveService.ApplyLeave(dto, userId, role);


        return Ok("Leave Applied Successfully");
    }

    [Authorize(Roles = "Employee,Manager")]
    [HttpGet("my-leaves")]
    public IActionResult GetMyLeaves()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return Ok(leaveService.GetMyLeaves(userId));
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public IActionResult GetAllLeaves()
    {
        return Ok(leaveService.GetAllLeaves());
    }

    [Authorize(Roles = "Admin,HR,Manager")]
    [HttpPut("{leaveId}/approve")]
    public IActionResult ApproveLeave(Guid leaveId,LeaveActionDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var role = User.FindFirst(ClaimTypes.Role)!.Value;

        leaveService.ApproveLeave(leaveId, userId, role, dto);

        return Ok("Leave Approved");
    }


    [Authorize(Roles = "Admin,HR,Manager")]
    [HttpPut("{leaveId}/reject")]
    public IActionResult RejectLeave(Guid leaveId,LeaveActionDto dto)
    {

        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var role = User.FindFirst(ClaimTypes.Role)!.Value;

        leaveService.RejectLeave(leaveId, userId, role, dto);

        return Ok("Leave Rejected");
    }



}
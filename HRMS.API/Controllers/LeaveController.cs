using System.Security.Claims;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Leave;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> ApplyLeave(ApplyLeaveDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var role = User.FindFirst(ClaimTypes.Role)!.Value;

        await leaveService.ApplyLeaveAsync(dto, userId, role);

        return Ok("Leave Applied Successfully");
    }

    [Authorize(Roles = "Employee,Manager")]
    [HttpGet("my-leaves")]
    public async Task<IActionResult> GetMyLeaves()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var leaves = await leaveService.GetMyLeavesAsync(userId);

        return Ok(leaves);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public async Task<IActionResult> GetAllLeaves()
    {
        var leaves = await leaveService.GetAllLeavesAsync();

        return Ok(leaves);
    }

    [Authorize(Roles = "Admin,HR,Manager")]
    [HttpPut("{leaveId}/approve")]
    public async Task<IActionResult> ApproveLeave(Guid leaveId, LeaveActionDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var role = User.FindFirst(ClaimTypes.Role)!.Value;

        await leaveService.ApproveLeaveAsync(leaveId, userId, role, dto);

        return Ok("Leave Approved");
    }

    [Authorize(Roles = "Admin,HR,Manager")]
    [HttpPut("{leaveId}/reject")]
    public async Task<IActionResult> RejectLeave(Guid leaveId, LeaveActionDto dto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var role = User.FindFirst(ClaimTypes.Role)!.Value;

        await leaveService.RejectLeaveAsync(leaveId, userId, role, dto);

        return Ok("Leave Rejected");
    }
}
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Leave;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/leave")]
[ApiController]
[Authorize]
public class LeaveController : ControllerBase
{
    private readonly ILeaveService leaveService;

    public LeaveController(ILeaveService leaveService)
    {
        this.leaveService = leaveService;
    }

    [HttpPost("apply")]
    public async Task<IActionResult> ApplyLeave([FromBody] ApplyLeaveDto dto, CancellationToken cancellationToken)
    {
        await leaveService.ApplyLeaveAsync(dto, cancellationToken);
        return Ok(new { Message = "Leave applied successfully." });
    }

    [HttpGet]
    [Authorize(Roles = "Admin,HR,Manager")]
    public async Task<IActionResult> GetLeaves([FromQuery] LeaveFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await leaveService.GetLeavesAsync(filter, cancellationToken);
        return Ok(result);
    }

    [HttpGet("my-leaves")]
    public async Task<IActionResult> GetMyLeaves([FromQuery] LeaveFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await leaveService.GetMyLeavesAsync(filter, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{leaveId:guid}")]
    public async Task<IActionResult> GetLeave(Guid leaveId, CancellationToken cancellationToken)
    {
        var result = await leaveService.GetLeaveAsync(leaveId, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{leaveId:guid}/approve")]
    [Authorize(Roles = "Admin,HR,Manager")]
    public async Task<IActionResult> ApproveLeave(Guid leaveId, [FromBody] LeaveActionDto dto, CancellationToken cancellationToken)
    {
        await leaveService.ApproveLeaveAsync(leaveId, dto, cancellationToken);
        return Ok(new { Message = "Leave approved successfully." });
    }

    [HttpPut("{leaveId:guid}/reject")]
    [Authorize(Roles = "Admin,HR,Manager")]
    public async Task<IActionResult> RejectLeave(Guid leaveId, [FromBody] LeaveActionDto dto, CancellationToken cancellationToken)
    {
        await leaveService.RejectLeaveAsync(leaveId, dto, cancellationToken);
        return Ok(new { Message = "Leave rejected successfully." });
    }

    [HttpPut("{leaveId:guid}/withdraw")]
    [Authorize(Roles = "Employee,Manager")]
    public async Task<IActionResult> WithdrawLeave(Guid leaveId, CancellationToken cancellationToken)
    {
        await leaveService.WithdrawLeaveAsync(leaveId, cancellationToken);
        return Ok(new { Message = "Leave withdrawn successfully." });
    }

    [HttpPut("{leaveId:guid}/cancel")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> CancelLeave(Guid leaveId, CancellationToken cancellationToken)
    {
        await leaveService.CancelLeaveAsync(leaveId, cancellationToken);
        return Ok(new { Message = "Leave cancelled successfully." });
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetLeaveBalances(CancellationToken cancellationToken)
    {
        var result = await leaveService.GetMyLeaveBalancesAsync(cancellationToken);
        return Ok(result);
    }
}
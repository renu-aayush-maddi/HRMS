using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.LeaveBalance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.API.Controllers;

[Route("api/leave-balances")]
[ApiController]
[Authorize]
public class LeaveBalancesController : ControllerBase
{
    private readonly ILeaveBalanceService service;

    public LeaveBalancesController(ILeaveBalanceService service)
    {
        this.service = service;
    }

    [Authorize(Roles = "Admin,HR,Manager,Employee")]
    [HttpPost("allocate")]
    public async Task<IActionResult> Allocate(AllocateLeaveBalanceDto dto, CancellationToken cancellationToken)
    {
        await service.AllocateAsync(dto, cancellationToken);

        return Ok("Leave Balance Allocated");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await service.GetAllBalancesAsync(cancellationToken));
    }

    [Authorize(Roles = "Admin,HR,Manager,Employee")]
    [HttpGet("{employeeId}")]
    public async Task<IActionResult> GetEmployeeBalances(Guid employeeId, CancellationToken cancellationToken)
    {
        return Ok(await service.GetEmployeeBalancesAsync(employeeId, cancellationToken));
    }
}
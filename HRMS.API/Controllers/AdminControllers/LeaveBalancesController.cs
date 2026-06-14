using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.LeaveBalance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [Authorize(Roles = "Admin,HR")]
    [HttpPost("allocate")]
    public async Task<IActionResult> Allocate(AllocateLeaveBalanceDto dto)
    {
        await service.AllocateAsync(dto);

        return Ok("Leave Balance Allocated");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await service.GetAllBalancesAsync());
    }

    
    [HttpGet("{employeeId}")]
    public async Task<IActionResult> GetEmployeeBalances(Guid employeeId)
    {
        return Ok(await service.GetEmployeeBalancesAsync(employeeId));
    }
}
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.LeaveBalance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/leave-balances")]
[ApiController]
[Authorize]
public class LeaveBalancesController: ControllerBase
{
    private readonly ILeaveBalanceService service;

    public LeaveBalancesController(ILeaveBalanceService service)
    {
        this.service = service;
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost("allocate")]
    public IActionResult Allocate(AllocateLeaveBalanceDto dto)
    {
        service.Allocate(dto);

        return Ok("Leave Balance Allocated");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(service.GetAllBalances());
    }

    [Authorize]
    [HttpGet("{employeeId}")]
    public IActionResult GetEmployeeBalances(Guid employeeId)
    {
        return Ok(service.GetEmployeeBalances(employeeId));
    }
}
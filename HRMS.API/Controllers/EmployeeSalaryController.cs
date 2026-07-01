using HRMS.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using HRMS.API.Models.DTOs.EmployeeSalary;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.API.Controllers;

[Route("api/employee-salaries")]
[ApiController]
[Authorize]
public class EmployeeSalaryController : ControllerBase
{
    private readonly IEmployeeSalaryService employeeSalaryService;

    public EmployeeSalaryController(IEmployeeSalaryService employeeSalaryService)
    {
        this.employeeSalaryService = employeeSalaryService;
    }

    [Authorize(Roles = "Admin,HR,Manager,Employee")]
    [HttpPost]
    public async Task<IActionResult> AssignSalary(AssignEmployeeSalaryDto dto, CancellationToken cancellationToken)
    {
        await employeeSalaryService.AssignSalaryAsync(dto, cancellationToken);

        return Ok("Salary Assigned Successfully");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await employeeSalaryService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{employeeId}")]
    public async Task<IActionResult> GetActiveSalary(Guid employeeId, CancellationToken cancellationToken)
    {
        return Ok(await employeeSalaryService.GetActiveSalaryAsync(employeeId, cancellationToken));
    }

    [Authorize(Roles = "Admin,HR,Manager,Employee")]
    [HttpGet("history/{employeeId}")]
    public async Task<IActionResult> GetSalaryHistory(Guid employeeId, CancellationToken cancellationToken)
    {
        return Ok(await employeeSalaryService.GetSalaryHistoryAsync(employeeId, cancellationToken));
    }
}
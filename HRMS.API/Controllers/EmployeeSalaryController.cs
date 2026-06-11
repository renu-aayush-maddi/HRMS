using HRMS.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using HRMS.API.Models.DTOs.EmployeeSalary;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;


[Route("api/employee-salaries")]
[ApiController]
[Authorize]
public class EmployeeSalaryController
    : ControllerBase
{
    private readonly IEmployeeSalaryService
        employeeSalaryService;

    public EmployeeSalaryController(
        IEmployeeSalaryService employeeSalaryService)
    {
        this.employeeSalaryService =
            employeeSalaryService;
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost]
    public IActionResult AssignSalary(
        AssignEmployeeSalaryDto dto)
    {
        employeeSalaryService
            .AssignSalary(dto);

        return Ok(
            "Salary Assigned Successfully");
    }


    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(
            employeeSalaryService
                .GetAll());
    }

    [HttpGet("{employeeId}")]
    public IActionResult GetActiveSalary(
        Guid employeeId)
    {
        return Ok(
            employeeSalaryService
                .GetActiveSalary(employeeId));
    }


    [Authorize(Roles = "Admin,HR")]
    [HttpGet("history/{employeeId}")]
    public IActionResult GetSalaryHistory(
        Guid employeeId)
    {
        return Ok(
            employeeSalaryService
                .GetSalaryHistory(
                    employeeId));
    }
}
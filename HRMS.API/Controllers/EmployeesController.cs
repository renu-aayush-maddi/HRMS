using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        this.employeeService = employeeService;
    }

    [Authorize(Roles = "Admin,HR,Manager")]
    [HttpGet]
    public IActionResult GetAllEmployees(string? search = null,
        int page = 1,
        int pageSize = 5)
    {
        var employees =
            employeeService.GetAllEmployees(
                search,
                page,
                pageSize);

        return Ok(employees);
    }

    [Authorize(Roles = "Admin,HR,Manager")]
    [HttpGet("{id}")]
    public IActionResult GetEmployeeById(Guid id)
    {
        var employee =
            employeeService.GetEmployeeById(id);

        if (employee == null)
        {
            return NotFound();
        }

        return Ok(employee);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost]
    public IActionResult AddEmployee(
        AddEmployeeDto dto)
    {
        var result = employeeService.AddEmployee(dto);

        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}")]
    public IActionResult UpdateEmployee(
        Guid id,
        UpdateEmployeeDto dto)
    {
        employeeService.UpdateEmployee(id, dto);

        return Ok("Employee Updated Successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public IActionResult DeleteEmployee(Guid id)
    {
        employeeService.DeleteEmployee(id);

        return Ok("Employee Deleted Successfully");
    }


    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}/status")]
    public IActionResult UpdateStatus(
        Guid id,
        UpdateEmployeeStatusDto dto)
    {
        employeeService
            .UpdateEmployeeStatus(id, dto);

        return Ok(
            "Employee Status Updated Successfully");
    }


    [Authorize(Roles = "Admin,HR,Manager")]
    [HttpGet("{employeeId}/full-profile")]
    public IActionResult GetFullProfile(
        Guid employeeId)
    {
        var result =
            employeeService
            .GetFullProfile(
                employeeId);

        if(result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
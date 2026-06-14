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

    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public async Task<IActionResult> GetAllEmployees(
        string? search = null,
        int page = 1,
        int pageSize = 5)
    {
        var employees = await employeeService.GetAllEmployeesAsync(search,page,pageSize);

        return Ok(employees);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployeeById(Guid id)
    {
        var employee =
            await employeeService.GetEmployeeByIdAsync(id);

        return Ok(employee);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost]
    public async Task<IActionResult> AddEmployee(AddEmployeeDto dto)
    {
        var result =
            await employeeService.AddEmployeeAsync(dto);

        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(Guid id,UpdateEmployeeDto dto)
    {
        await employeeService.UpdateEmployeeAsync(id, dto);

        return Ok("Employee Updated Successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(Guid id)
    {
        await employeeService.DeleteEmployeeAsync(id);

        return Ok("Employee Deleted Successfully");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id,UpdateEmployeeStatusDto dto)
    {
        await employeeService.UpdateEmployeeStatusAsync(id,dto);

        return Ok("Employee Status Updated Successfully");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("{employeeId}/full-profile")]
    public async Task<IActionResult> GetFullProfile(Guid employeeId)
    {
        var result =
            await employeeService.GetFullProfileAsync(employeeId);

        return Ok(result);
    }
}
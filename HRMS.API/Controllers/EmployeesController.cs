using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        this.employeeService = employeeService;
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost]
    public async Task<IActionResult> AddEmployee([FromBody] AddEmployeeDto dto, CancellationToken cancellationToken)
    {
        var result = await employeeService.AddEmployeeAsync(dto, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{employeeId:guid}")]
    public async Task<IActionResult> UpdateEmployee(Guid employeeId, [FromBody] UpdateEmployeeDto dto, CancellationToken cancellationToken)
    {
        var result = await employeeService.UpdateEmployeeAsync(employeeId, dto, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpDelete("{employeeId:guid}")]
    public async Task<IActionResult> DeleteEmployee(Guid employeeId, CancellationToken cancellationToken)
    {
        await employeeService.DeleteEmployeeAsync(employeeId, cancellationToken);
        return Ok("Employee deleted successfully.");
    }

    [Authorize(Roles = "Admin,HR,Manager")]
    [HttpGet("{employeeId:guid}")]
    public async Task<IActionResult> GetEmployeeById(Guid employeeId, CancellationToken cancellationToken)
    {
        var result = await employeeService.GetEmployeeByIdAsync(employeeId, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR,Manager")]
    [HttpGet]
    public async Task<IActionResult> GetEmployees([FromQuery] EmployeeFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await employeeService.GetEmployeesAsync(filter, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR,Manager")]
    [HttpGet("{employeeId:guid}/profile")]
    public async Task<IActionResult> GetEmployeeProfile(Guid employeeId, CancellationToken cancellationToken)
    {
        var result = await employeeService.GetEmployeeFullProfileAsync(employeeId, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("my-profile")]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var result = await employeeService.GetMyProfileAsync(cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{employeeId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid employeeId, [FromBody] UpdateEmployeeStatusDto dto, CancellationToken cancellationToken)
    {
        await employeeService.UpdateEmployeeStatusAsync(employeeId, dto, cancellationToken);
        return Ok("Employee status updated successfully.");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("managers")]
    public async Task<IActionResult> GetManagers(CancellationToken cancellationToken)
    {
        var result = await employeeService.GetManagersAsync(cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("export")]
    public async Task<IActionResult> ExportEmployees([FromQuery] EmployeeFilterDto filter, CancellationToken cancellationToken)
    {
        var file = await employeeService.ExportEmployeesAsync(filter, cancellationToken);
        return File(
            file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"employees-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ImportEmployees(IFormFile file, CancellationToken cancellationToken)
    {
        var result = await employeeService.ImportEmployeesAsync(file, cancellationToken);
        return Ok(result);
    }
}
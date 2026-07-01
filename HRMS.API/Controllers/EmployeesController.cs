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

    [Authorize(Roles = "Admin,HR,Manager,Employee")]
    [HttpGet("{employeeId:guid}/profile")]
    public async Task<IActionResult> GetEmployeeProfile(Guid employeeId, CancellationToken cancellationToken)
    {
        var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        if (userRole == "Employee")
        {
            var loggedInEmployeeIdClaim = User.FindFirst("EmployeeId")?.Value;
            if (string.IsNullOrEmpty(loggedInEmployeeIdClaim) || Guid.Parse(loggedInEmployeeIdClaim) != employeeId)
            {
                return Forbid();
            }
        }

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
    [HttpPatch("{employeeId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid employeeId,
        [FromBody] UpdateEmployeeStatusDto dto,
        CancellationToken cancellationToken)
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
            $"employees-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ImportEmployees(IFormFile file, CancellationToken cancellationToken)
    {
        var result = await employeeService.ImportEmployeesAsync(file, cancellationToken);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("my-profile")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileDto dto, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        await employeeService.UpdateMyProfileAsync(userId, dto, cancellationToken);
        return Ok(new { Message = "Profile updated successfully." });
    }

    [Authorize]
    [HttpPost("my-profile/photo")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadProfilePhoto(IFormFile file, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        var photoUrl = await employeeService.UploadProfilePhotoAsync(userId, file, cancellationToken);
        return Ok(new { ProfilePhotoUrl = photoUrl, Message = "Profile photo uploaded successfully." });
    }

    [Authorize]
    [HttpDelete("my-profile/photo")]
    public async Task<IActionResult> DeleteProfilePhoto(CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        await employeeService.DeleteProfilePhotoAsync(userId, cancellationToken);
        return Ok(new { Message = "Profile photo deleted successfully." });
    }
}
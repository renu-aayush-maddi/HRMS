using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.EmployeeEducation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;


[Route("api/employee-educations")]
[ApiController]
[Authorize]
public class EmployeeEducationsController : ControllerBase
{
    private readonly IEmployeeEducationService service;

    public EmployeeEducationsController(IEmployeeEducationService service)
    {
        this.service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddEducation([FromBody] AddEmployeeEducationDto dto, CancellationToken cancellationToken)
    {
        var result = await service.AddEducationAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetEducations), new { employeeId = result.EmployeeId }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetEducations([FromQuery] EmployeeEducationFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await service.GetEducationsAsync(filter, cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateEducation(Guid id, [FromBody] UpdateEmployeeEducationDto dto, CancellationToken cancellationToken)
    {
        var result = await service.UpdateEducationAsync(id, dto, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEducation(Guid id, CancellationToken cancellationToken)
    {
        await service.DeleteEducationAsync(id, cancellationToken);

        return Ok(new ApiResponse
        {
            Message = "Education deleted successfully."
        });
    }

    [Authorize(Roles = "Admin,HR,Manager,Employee")]
    [HttpGet("export")]
    public async Task<IActionResult> ExportEducations([FromQuery] EmployeeEducationFilterDto filter, CancellationToken cancellationToken)
    {
        var fileBytes = await service.ExportEducationsAsync(filter, cancellationToken);

        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"employee-educations-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }

    [Authorize(Roles = "Admin,HR,Manager,Employee")]
    [HttpPost("import")]
    public async Task<IActionResult> ImportEducations(IFormFile file, CancellationToken cancellationToken)
    {
        var result = await service.ImportEducationsAsync(file, cancellationToken);

        return Ok(result);
    }
}
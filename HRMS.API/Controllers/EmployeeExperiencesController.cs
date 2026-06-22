using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.EmployeeExperience;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;


[Route("api/employee-experiences")]
[ApiController]
[Authorize]
public class EmployeeExperiencesController : ControllerBase
{
    private readonly IEmployeeExperienceService service;

    public EmployeeExperiencesController(IEmployeeExperienceService service)
    {
        this.service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddExperience([FromBody] AddEmployeeExperienceDto dto, CancellationToken cancellationToken)
    {
        var result = await service.AddExperienceAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetExperiences), new { employeeId = result.EmployeeId }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetExperiences([FromQuery] EmployeeExperienceFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await service.GetExperiencesAsync(filter, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateExperience(Guid id, [FromBody] UpdateEmployeeExperienceDto dto, CancellationToken cancellationToken)
    {
        var result = await service.UpdateExperienceAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteExperience(Guid id, CancellationToken cancellationToken)
    {
        await service.DeleteExperienceAsync(id, cancellationToken);
        return Ok(new ApiResponse { Message = "Experience deleted successfully." });
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("export")]
    public async Task<IActionResult> ExportExperiences([FromQuery] EmployeeExperienceFilterDto filter, CancellationToken cancellationToken)
    {
        var fileBytes = await service.ExportExperiencesAsync(filter, cancellationToken);
        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"employee-experiences-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost("import")]
    public async Task<IActionResult> ImportExperiences(IFormFile file, CancellationToken cancellationToken)
    {
        var result = await service.ImportExperiencesAsync(file, cancellationToken);
        return Ok(result);
    }
}
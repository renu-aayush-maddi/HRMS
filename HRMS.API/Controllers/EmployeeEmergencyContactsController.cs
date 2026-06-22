using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.EmployeeEmergencyContact;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/employee-emergency-contacts")]
[ApiController]
[Authorize]

public class EmployeeEmergencyContactsController : ControllerBase
{
    private readonly IEmployeeEmergencyContactService service;

    public EmployeeEmergencyContactsController(IEmployeeEmergencyContactService service)
    {
        this.service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddContact([FromBody] AddEmployeeEmergencyContactDto dto, CancellationToken cancellationToken)
    {
        var result = await service.AddContactAsync(dto, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetContacts([FromQuery] EmployeeEmergencyContactFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await service.GetContactsAsync(filter, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateContact(Guid id, [FromBody] UpdateEmployeeEmergencyContactDto dto, CancellationToken cancellationToken)
    {
        var result = await service.UpdateContactAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteContact(Guid id, CancellationToken cancellationToken)
    {
        await service.DeleteContactAsync(id, cancellationToken);
        return Ok(new ApiResponse { Message = "Emergency contact deleted successfully." });
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("export")]
    public async Task<IActionResult> ExportContacts([FromQuery] EmployeeEmergencyContactFilterDto filter, CancellationToken cancellationToken)
    {
        var file = await service.ExportContactsAsync(filter, cancellationToken);
        return File(
            file, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            $"employee-emergency-contacts-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost("import")]
    public async Task<IActionResult> ImportContacts(IFormFile file, CancellationToken cancellationToken)
    {
        var result = await service.ImportContactsAsync(file, cancellationToken);
        return Ok(result);
    }
}
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.AttendanceRegularization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/attendance-regularizations")]
[ApiController]
[Authorize]
public class AttendanceRegularizationsController : ControllerBase
{
    private readonly IAttendanceRegularizationService service;

    public AttendanceRegularizationsController(IAttendanceRegularizationService service)
    {
        this.service = service;
    }

    [Authorize(Roles = "Employee,Manager,HR")]
    [HttpPost]
    public async Task<IActionResult> CreateRequest([FromBody] CreateAttendanceRegularizationDto dto, CancellationToken cancellationToken)
    {
        var result = await service.CreateRequestAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetRequests), new { employeeId = result.EmployeeId }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetRequests([FromQuery] AttendanceRegularizationFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await service.GetRequestsAsync(filter, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ApproveAttendanceRegularizationDto dto, CancellationToken cancellationToken)
    {
        var result = await service.ApproveAsync(id, dto, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, [FromBody] RejectAttendanceRegularizationDto dto, CancellationToken cancellationToken)
    {
        var result = await service.RejectAsync(id, dto, cancellationToken);
        return Ok(result);
    }
    
}
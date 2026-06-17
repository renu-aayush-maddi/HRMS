using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Attendance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/attendance")]
[ApiController]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService service;

    public AttendanceController(IAttendanceService service)
    {
        this.service = service;
    }

    [Authorize(Roles = "Employee,Manager,HR")]
    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn(CancellationToken cancellationToken)
    {
        await service.CheckInAsync(cancellationToken);
        return Ok("Checked in successfully.");
    }

    [Authorize(Roles = "Employee,Manager,HR")]
    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut(CancellationToken cancellationToken)
    {
        await service.CheckOutAsync(cancellationToken);
        return Ok("Checked out successfully.");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public async Task<IActionResult> GetAttendance([FromQuery] AttendanceFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await service.GetAttendanceAsync(filter, cancellationToken);
        return Ok(result);
    }

    [HttpGet("employee")]
    public async Task<IActionResult> GetEmployeeAttendance([FromQuery] AttendanceFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await service.GetEmployeeAttendanceAsync(filter, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("export")]
    public async Task<IActionResult> ExportAttendance([FromQuery] AttendanceFilterDto filter, CancellationToken cancellationToken)
    {
        var fileBytes = await service.ExportAttendanceAsync(filter, cancellationToken);
        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"attendance-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }
}
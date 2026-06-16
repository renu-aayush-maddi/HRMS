using System.Security.Claims;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Attendance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService attendanceService;

    public AttendanceController(IAttendanceService attendanceService)
    {
        this.attendanceService = attendanceService;
    }

    [Authorize(Roles = "Employee,HR,Manager")]
    [HttpPost("checkin")]
    public async Task<IActionResult> CheckIn()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        await attendanceService.CheckInAsync(userId);

        return Ok("Checked In Successfully");
    }

    [Authorize(Roles = "Employee,HR,Manager")]
    [HttpPost("checkout")]
    public async Task<IActionResult> CheckOut()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        await attendanceService.CheckOutAsync(userId);

        return Ok("Checked Out Successfully");
    }

    [Authorize(Roles = "Admin,HR,Manager")]
    [HttpGet]
    public async Task<IActionResult> GetAttendance([FromQuery] AttendanceQueryDto query)
    {
        var result = await attendanceService.GetAttendanceAsync(query);

        return Ok(result);
    }

    [Authorize(Roles = "Employee,HR,Manager,Admin")]
    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetEmployeeAttendance(Guid employeeId)
    {
    
        var result = await attendanceService.GetEmployeeAttendanceAsync(employeeId);

        return Ok(result);
    }
}
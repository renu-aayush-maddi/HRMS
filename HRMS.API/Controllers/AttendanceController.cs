using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Attendance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HRMS.API.Models.DTOs.Common;

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
    public IActionResult CheckIn()
    {
        var userId =Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        attendanceService.CheckIn(userId);

        return Ok("Checked In Successfully");
    }


    [Authorize(Roles = "Employee,HR,Manager")]
    [HttpPost("checkout")]
    public IActionResult CheckOut()
    {
        var userId =Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        attendanceService.CheckOut(userId);

        return Ok("Checked Out Successfully");
    }

    [Authorize(Roles = "Admin,HR,Manager")]
    [HttpGet]
    public IActionResult GetAttendance([FromQuery] AttendanceQueryDto query)
    {
        return Ok(attendanceService.GetAttendance(query));
    }

    [Authorize(Roles = "Employee,HR,Manager,Admin")]
    [HttpGet("employee/{employeeId}")]
    public IActionResult GetEmployeeAttendance(Guid employeeId)
    {
        return Ok(attendanceService.GetEmployeeAttendance(employeeId));
    }
}
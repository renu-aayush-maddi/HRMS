using HRMS.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/hr-dashboard")]
[ApiController]
[Authorize]
public class HrDashboardController : ControllerBase
{
    private readonly IHrDashboardService service;

    public HrDashboardController(IHrDashboardService service)
    {
        this.service = service;
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        return Ok(service.GetStats());
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("department-summary")]
    public IActionResult GetDepartmentSummary()
    {
        return Ok(service.GetDepartmentSummary());
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("leave-summary")]
    public IActionResult GetLeaveSummary()
    {
        return Ok(service.GetLeaveSummary());
    }
}
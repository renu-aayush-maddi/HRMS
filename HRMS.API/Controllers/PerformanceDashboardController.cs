using HRMS.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.API.Controllers;

[Route("api/performance/dashboard")]
[ApiController]
[Authorize]
public class PerformanceDashboardController
    : ControllerBase
{
    private readonly
        IPerformanceDashboardService
        service;

    public PerformanceDashboardController(
        IPerformanceDashboardService service)
    {
        this.service = service;
    }

    [Authorize(Roles = "Manager")]
    [HttpGet("manager")]
    public IActionResult GetManagerDashboard(
        [FromQuery] Guid cycleId)
    {
        var managerUserId =
            Guid.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!
                .Value);

        return Ok(
            service.GetManagerDashboard(
                managerUserId,
                cycleId));
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("hr")]
    public IActionResult GetHrDashboard(
        [FromQuery] Guid cycleId)
    {
        return Ok(
            service.GetHrDashboard(
                cycleId));
    }
}
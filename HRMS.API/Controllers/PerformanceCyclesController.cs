using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.PerformanceCycle;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/performance-cycles")]
[ApiController]
[Authorize]
public class PerformanceCyclesController : ControllerBase
{
    private readonly IPerformanceCycleService service;

    public PerformanceCyclesController(IPerformanceCycleService service)
    {
        this.service = service;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HR")]
    public IActionResult AddCycle(AddPerformanceCycleDto dto)
    {
        service.AddCycle(dto);
        return Ok("Cycle Added Successfully");
    }

    [HttpGet]
    public IActionResult GetAllCycles()
    {
        return Ok(service.GetAllCycles());
    }

    [HttpGet("{id}")]
    public IActionResult GetCycle(Guid id)
    {
        return Ok(service.GetCycleById(id));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public IActionResult UpdateCycle(Guid id, UpdatePerformanceCycleDto dto)
    {
        service.UpdateCycle(id, dto);
        return Ok("Cycle Updated Successfully");
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,HR")]
    public IActionResult DeleteCycle(Guid id)
    {
        service.DeleteCycle(id);
        return Ok("Cycle Deleted Successfully");
    }
}
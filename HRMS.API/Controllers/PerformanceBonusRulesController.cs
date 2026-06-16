using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.PerformanceBonusRule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/performance-bonus-rules")]
[ApiController]
[Authorize(Roles = "Admin,HR")]
public class PerformanceBonusRulesController : ControllerBase
{
    private readonly IPerformanceBonusRuleService service;

    public PerformanceBonusRulesController(IPerformanceBonusRuleService service)
    {
        this.service = service;
    }

    [HttpPost]
    public IActionResult AddRule(AddPerformanceBonusRuleDto dto)
    {
        service.AddRule(dto);

        return Ok("Rule Added Successfully");
    }

    [HttpGet]
    public IActionResult GetAllRules()
    {
        return Ok(service.GetAllRules());
    }

    [HttpGet("{id}")]
    public IActionResult GetRule(Guid id)
    {
        return Ok(service.GetRuleById(id));
    }

    [HttpPut("{id}")]
    public IActionResult UpdateRule(Guid id, UpdatePerformanceBonusRuleDto dto)
    {
        service.UpdateRule(id, dto);

        return Ok("Rule Updated Successfully");
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteRule(Guid id)
    {
        service.DeleteRule(id);

        return Ok("Rule Deleted Successfully");
    }
}
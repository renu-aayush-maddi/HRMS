using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Deduction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.API.Controllers;

[Route("api/deductions")]
[ApiController]
[Authorize]
public class DeductionController : ControllerBase
{
    private readonly IDeductionService deductionService;

    public DeductionController(IDeductionService deductionService)
    {
        this.deductionService = deductionService;
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost]
    public IActionResult CreateDeduction(CreateDeductionDto dto)
    {
        deductionService.CreateDeduction(dto);

        return Ok("Deduction Created Successfully");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(deductionService.GetAllDeductions());
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{deductionId}/approve")]
    public IActionResult ApproveDeduction(Guid deductionId)
    {
        deductionService.ApproveDeduction(deductionId);

        return Ok("Deduction Approved");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{deductionId}/reject")]
    public IActionResult RejectDeduction(Guid deductionId)
    {
        deductionService.RejectDeduction(deductionId);

        return Ok("Deduction Rejected");
    }

    [HttpGet("my-deductions")]
    public IActionResult GetMyDeductions()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return Ok(deductionService.GetMyDeductions(userId));
    }
}
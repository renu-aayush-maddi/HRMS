using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Deduction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/deductions")]
[ApiController]
[Authorize]
public class DeductionController : ControllerBase
{
    private readonly IDeductionService deductionService;

    public DeductionController(
        IDeductionService deductionService)
    {
        this.deductionService = deductionService;
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost]
    public async Task<IActionResult> CreateDeduction(
        [FromBody] CreateDeductionDto dto,
        CancellationToken cancellationToken)
    {
        var result =
            await deductionService.CreateDeductionAsync(
                dto,
                cancellationToken);

        return CreatedAtAction(
            nameof(GetDeduction),
            new { deductionId = result.Id },
            result);
    }

    [HttpGet]
    public async Task<IActionResult> GetDeductions(
        [FromQuery] DeductionFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result =
            await deductionService.GetDeductionsAsync(
                filter,
                cancellationToken);

        return Ok(result);
    }

    [HttpGet("{deductionId:guid}")]
    public async Task<IActionResult> GetDeduction(
        Guid deductionId,
        CancellationToken cancellationToken)
    {
        var result =
            await deductionService.GetDeductionAsync(
                deductionId,
                cancellationToken);

        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyDeductions(
        [FromQuery] DeductionFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result =
            await deductionService.GetMyDeductionsAsync(
                filter,
                cancellationToken);

        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{deductionId:guid}/approve")]
    public async Task<IActionResult> ApproveDeduction(
        Guid deductionId,
        CancellationToken cancellationToken)
    {
        await deductionService.ApproveDeductionAsync(
            deductionId,
            cancellationToken);

        return Ok(new
        {
            Message = "Deduction approved successfully."
        });
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{deductionId:guid}/reject")]
    public async Task<IActionResult> RejectDeduction(
        Guid deductionId,
        CancellationToken cancellationToken)
    {
        await deductionService.RejectDeductionAsync(
            deductionId,
            cancellationToken);

        return Ok(new
        {
            Message = "Deduction rejected successfully."
        });
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("export")]
    public async Task<IActionResult> ExportDeductions(
        [FromQuery] DeductionFilterDto filter,
        CancellationToken cancellationToken)
    {
        var fileBytes =
            await deductionService.ExportDeductionsAsync(
                filter,
                cancellationToken);

        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"deductions-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }
}
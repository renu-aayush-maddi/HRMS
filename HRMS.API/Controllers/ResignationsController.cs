using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Resignation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[ApiController]
[Route("api/employee-resignations")]
[Authorize]
public class EmployeeResignationsController : ControllerBase
{
    private readonly IEmployeeResignationService resignationService;

    public EmployeeResignationsController(IEmployeeResignationService resignationService)
    {
        this.resignationService = resignationService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateResignationDto dto, CancellationToken cancellationToken)
    {
        var result = await resignationService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { resignationId = result.Id }, result);
    }

    [HttpGet("{resignationId:guid}")]
    public async Task<IActionResult> GetById(Guid resignationId, CancellationToken cancellationToken)
    {
        var result = await resignationService.GetByIdAsync(resignationId, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ResignationFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await resignationService.GetAllAsync(filter, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{resignationId:guid}/approve")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Approve(Guid resignationId, CancellationToken cancellationToken)
    {
        await resignationService.ApproveAsync(resignationId, cancellationToken);
        return NoContent();
    }

    [HttpPut("{resignationId:guid}/reject")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Reject(Guid resignationId, RejectResignationDto dto, CancellationToken cancellationToken)
    {
        await resignationService.RejectAsync(resignationId, dto, cancellationToken);
        return NoContent();
    }

    [HttpPut("{resignationId:guid}/withdraw")]
    public async Task<IActionResult> Withdraw(Guid resignationId, CancellationToken cancellationToken)
    {
        await resignationService.WithdrawAsync(resignationId, cancellationToken);
        return NoContent();
    }

    [HttpPut("{resignationId:guid}/settlement-status")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> UpdateSettlementStatus(Guid resignationId, UpdateSettlementStatusDto dto, CancellationToken cancellationToken)
    {
        await resignationService.UpdateSettlementStatusAsync(resignationId, dto, cancellationToken);
        return NoContent();
    }

    [HttpGet("export")]
    [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Export([FromQuery] ResignationFilterDto filter, CancellationToken cancellationToken)
    {
        var fileBytes = await resignationService.ExportAsync(filter, cancellationToken);
        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"resignations-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }
}
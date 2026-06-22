using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Payroll;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/payroll")]
[ApiController]
[Authorize]
public class PayrollController : ControllerBase
{
    private readonly IPayrollService payrollService;

    public PayrollController(IPayrollService payrollService)
    {
        this.payrollService = payrollService;
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost("generate")]
    public async Task<IActionResult> GeneratePayroll([FromBody] GeneratePayrollDto dto, CancellationToken cancellationToken)
    {
        var result = await payrollService.GeneratePayrollAsync(dto, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost("generate-monthly")]
    public async Task<IActionResult> GenerateMonthlyPayroll([FromBody] GenerateMonthlyPayrollDto dto, CancellationToken cancellationToken)
    {
        var result = await payrollService.GenerateMonthlyPayrollAsync(dto, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public async Task<IActionResult> GetPayrolls([FromQuery] PayrollFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await payrollService.GetPayrollsAsync(filter, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("employee/{employeeId:guid}")]
    public async Task<IActionResult> GetEmployeePayrolls(Guid employeeId, [FromQuery] PayrollFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await payrollService.GetEmployeePayrollsAsync(employeeId, filter, cancellationToken);
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyPayrolls([FromQuery] PayrollFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await payrollService.GetMyPayrollsAsync(filter, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{payrollId:guid}")]
    public async Task<IActionResult> GetPayroll(Guid payrollId, CancellationToken cancellationToken)
    {
        var result = await payrollService.GetPayrollAsync(payrollId, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{payrollId:guid}/approve")]
    public async Task<IActionResult> ApprovePayroll(Guid payrollId, CancellationToken cancellationToken)
    {
        await payrollService.ApprovePayrollAsync(payrollId, cancellationToken);
        return Ok(new { Message = "Payroll approved successfully." });
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{payrollId:guid}/mark-paid")]
    public async Task<IActionResult> MarkPayrollPaid(Guid payrollId, CancellationToken cancellationToken)
    {
        await payrollService.MarkPayrollPaidAsync(payrollId, cancellationToken);
        return Ok(new { Message = "Payroll marked as paid successfully." });
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("export")]
    public async Task<IActionResult> ExportPayrolls([FromQuery] PayrollFilterDto filter, CancellationToken cancellationToken)
    {
        var fileBytes = await payrollService.ExportPayrollsAsync(filter, cancellationToken);
        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"payrolls-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }
}
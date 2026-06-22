using HRMS.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/payslips")]
[ApiController]
[Authorize]
public class PayslipController : ControllerBase
{
    private readonly IPayslipService payslipService;

    public PayslipController(
        IPayslipService payslipService)
    {
        this.payslipService = payslipService;
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("{payrollId:guid}")]
    public async Task<IActionResult> GeneratePayslip(
        Guid payrollId,
        CancellationToken cancellationToken)
    {
        var pdf =
            await payslipService.GeneratePayslipAsync(
                payrollId,
                cancellationToken);

        return File(
            pdf,
            "application/pdf",
            $"Payslip-{payrollId}.pdf");
    }

    [HttpGet("my/{payrollId:guid}")]
    public async Task<IActionResult> GenerateMyPayslip(
        Guid payrollId,
        CancellationToken cancellationToken)
    {
        var pdf =
            await payslipService.GenerateMyPayslipAsync(
                payrollId,
                cancellationToken);

        return File(
            pdf,
            "application/pdf",
            $"Payslip-{payrollId}.pdf");
    }
}
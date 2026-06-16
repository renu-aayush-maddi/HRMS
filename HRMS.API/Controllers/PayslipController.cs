using HRMS.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.API.Controllers;

[Route("api/payslips")]
[ApiController]
[Authorize]
public class PayslipController : ControllerBase
{
    private readonly IPayslipService payslipService;

    public PayslipController(IPayslipService payslipService)
    {
        this.payslipService = payslipService;
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("{payrollId}")]
    public IActionResult GeneratePayslip(Guid payrollId)
    {
        var pdf = payslipService.GeneratePayslip(payrollId);

        return File(pdf, "application/pdf", $"Payslip-{payrollId}.pdf");
    }

    [HttpGet("my/{payrollId}")]
    public IActionResult GenerateMyPayslip(Guid payrollId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var pdf = payslipService.GenerateMyPayslip(payrollId, userId);

        return File(pdf, "application/pdf", $"Payslip-{payrollId}.pdf");
    }
}
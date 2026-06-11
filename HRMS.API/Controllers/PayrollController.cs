using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Payroll;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PayrollController : ControllerBase
{
    private readonly IPayrollService payrollService;

    public PayrollController(
        IPayrollService payrollService)
    {
        this.payrollService = payrollService;
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost("generate")]
    public IActionResult GeneratePayroll(
        GeneratePayrollDto dto)
    {
        payrollService.GeneratePayroll(dto);

        return Ok("Payroll Generated Successfully");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public IActionResult GetAllPayrolls()
    {
        return Ok(
            payrollService.GetAllPayrolls());
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("employee/{employeeId}")]
    public IActionResult GetEmployeePayrolls(
        Guid employeeId)
    {
        return Ok(
            payrollService
            .GetEmployeePayrolls(employeeId));
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{payrollId}/approve")]
    public IActionResult ApprovePayroll(
        Guid payrollId)
    {
        payrollService.ApprovePayroll(
            payrollId);

        return Ok(
            "Payroll Approved Successfully");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{payrollId}/mark-paid")]
    public IActionResult MarkPayrollPaid(
        Guid payrollId)
    {
        payrollService.MarkPayrollPaid(
            payrollId);

        return Ok(
            "Payroll Marked As Paid");
    }

    [Authorize(Roles = "Employee,Manager")]
    [HttpGet("my-payrolls")]
    public IActionResult GetMyPayrolls()
    {
        var userId =
            Guid.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!
                .Value);

        return Ok(
            payrollService
                .GetMyPayrolls(userId));
    }
}
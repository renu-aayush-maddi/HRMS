using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Resignation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/resignations")]
[ApiController]
[Authorize]
public class ResignationsController : ControllerBase
{
    private readonly IResignationService service;

    public ResignationsController(IResignationService service)
    {
        this.service = service;
    }

    [Authorize(Roles = "Employee , Admin , HR")]
    [HttpPost]
    public IActionResult SubmitResignation(SubmitResignationDto dto)
    {
        service.SubmitResignation(dto);

        return Ok("Resignation Submitted Successfully");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(service.GetAll());
    }

    [Authorize]
    [HttpGet("my/{employeeId}")]
    public IActionResult GetEmployeeResignations(Guid employeeId)
    {
        return Ok(service.GetEmployeeResignations(employeeId));
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}/approve")]
    public IActionResult Approve(
        Guid id,
        ResignationActionDto dto)
    {
        service.Approve(id,dto);

        return Ok("Resignation Approved");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}/reject")]
    public IActionResult Reject(Guid id,ResignationActionDto dto)
    {
        service.Reject(id,dto);

        return Ok("Resignation Rejected");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}/settlement")]
    public IActionResult UpdateSettlement(Guid id,SettlementDto dto)
    {
        service.UpdateSettlement(id,dto);

        return Ok("Settlement Updated");
    }
}
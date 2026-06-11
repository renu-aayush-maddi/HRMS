using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.LeaveType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/leave-types")]
[ApiController]
[Authorize]
public class LeaveTypesController: ControllerBase
{
    private readonly ILeaveTypeService service;

    public LeaveTypesController(ILeaveTypeService service)
    {
        this.service = service;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(service.GetAll());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Add(AddLeaveTypeDto dto)
    {
        service.Add(dto);

        return Ok("Leave type created");
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public IActionResult Update(
        Guid id,
        UpdateLeaveTypeDto dto)
    {
        service.Update(id, dto);

        return Ok("Leave type updated");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        service.Delete(id);

        return Ok("Leave type deleted");
    }
}
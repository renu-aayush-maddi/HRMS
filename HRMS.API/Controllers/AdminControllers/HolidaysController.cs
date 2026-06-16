using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Holiday;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/holidays")]
[ApiController]
[Authorize]
public class HolidaysController : ControllerBase
{
    private readonly IHolidayService service;

    public HolidaysController(IHolidayService service)
    {
        this.service = service;
    }


    [Authorize(Roles = "Admin,HR,Employee,Manager")]
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(service.GetAll());
    }

    [Authorize(Roles = "Admin,HR,Employee,Manager")]
    [HttpGet("upcoming")]
    public IActionResult GetUpcoming()
    {
        return Ok(service.GetUpcoming());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult AddHoliday(AddHolidayDto dto)
    {
        service.AddHoliday(dto);

        return Ok("Holiday Added Successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public IActionResult UpdateHoliday(Guid id, UpdateHolidayDto dto)
    {
        service.UpdateHoliday(id, dto);

        return Ok("Holiday Updated Successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public IActionResult DeleteHoliday(Guid id)
    {
        service.DeleteHoliday(id);

        return Ok("Holiday Deleted Successfully");
    }
}
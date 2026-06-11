using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.EmployeeExperience;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/employee-experiences")]
[ApiController]
[Authorize]
public class EmployeeExperiencesController
    : ControllerBase
{
    private readonly
        IEmployeeExperienceService service;

    public EmployeeExperiencesController(
        IEmployeeExperienceService service)
    {
        this.service = service;
    }

    [Authorize(Roles = "Admin,HR,Employee")]
    [HttpPost]
    public IActionResult AddExperience(
        AddEmployeeExperienceDto dto)
    {
        service.AddExperience(dto);

        return Ok(
            "Experience Added Successfully");
    }

    [HttpGet("{employeeId}")]
    public IActionResult GetExperiences(
        Guid employeeId)
    {
        return Ok(
            service.GetEmployeeExperiences(
                employeeId));
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}")]
    public IActionResult UpdateExperience(
        Guid id,
        UpdateEmployeeExperienceDto dto)
    {
        service.UpdateExperience(
            id,
            dto);

        return Ok(
            "Experience Updated Successfully");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpDelete("{id}")]
    public IActionResult DeleteExperience(
        Guid id)
    {
        service.DeleteExperience(id);

        return Ok(
            "Experience Deleted Successfully");
    }
}
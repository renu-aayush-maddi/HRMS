using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.EmployeeEducation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/employee-educations")]
[ApiController]
[Authorize]
public class EmployeeEducationsController
    : ControllerBase
{
    private readonly
        IEmployeeEducationService service;

    public EmployeeEducationsController(
        IEmployeeEducationService service)
    {
        this.service = service;
    }

    [Authorize(Roles = "Admin,HR,Employee")]
    [HttpPost]
    public IActionResult AddEducation(
        AddEmployeeEducationDto dto)
    {
        service.AddEducation(dto);

        return Ok(
            "Education Added Successfully");
    }

    [HttpGet("{employeeId}")]
    public IActionResult GetEducations(
        Guid employeeId)
    {
        return Ok(
            service.GetEmployeeEducations(
                employeeId));
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}")]
    public IActionResult UpdateEducation(
        Guid id,
        UpdateEmployeeEducationDto dto)
    {
        service.UpdateEducation(
            id,
            dto);

        return Ok(
            "Education Updated Successfully");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpDelete("{id}")]
    public IActionResult DeleteEducation(
        Guid id)
    {
        service.DeleteEducation(id);

        return Ok(
            "Education Deleted Successfully");
    }
}
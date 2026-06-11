using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.EmployeeEmergencyContact;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/employee-emergency-contacts")]
[ApiController]
[Authorize]
public class EmployeeEmergencyContactsController
    : ControllerBase
{
    private readonly
        IEmployeeEmergencyContactService service;

    public EmployeeEmergencyContactsController(
        IEmployeeEmergencyContactService service)
    {
        this.service = service;
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost]
    public IActionResult AddContact(
        AddEmployeeEmergencyContactDto dto)
    {
        service.AddContact(dto);

        return Ok(
            "Emergency Contact Added Successfully");
    }

    [HttpGet("{employeeId}")]
    public IActionResult GetContacts(
        Guid employeeId)
    {
        return Ok(
            service.GetEmployeeContacts(
                employeeId));
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}")]
    public IActionResult UpdateContact(
        Guid id,
        UpdateEmployeeEmergencyContactDto dto)
    {
        service.UpdateContact(
            id,
            dto);

        return Ok(
            "Emergency Contact Updated Successfully");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpDelete("{id}")]
    public IActionResult DeleteContact(
        Guid id)
    {
        service.DeleteContact(id);

        return Ok(
            "Emergency Contact Deleted Successfully");
    }
}
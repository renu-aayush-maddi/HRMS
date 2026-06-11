using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.EmployeeAddress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/employee-addresses")]
[ApiController]
[Authorize]
public class EmployeeAddressesController: ControllerBase
{
    private readonly IEmployeeAddressService service;

    public EmployeeAddressesController(IEmployeeAddressService service)
    {
        this.service = service;
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost]
    public IActionResult AddAddress(AddEmployeeAddressDto dto)
    {
        service.AddAddress(dto);

        return Ok("Address Added Successfully");
    }

    [HttpGet("{employeeId}")]
    public IActionResult GetAddresses(Guid employeeId)
    {
        return Ok(service.GetEmployeeAddresses(employeeId));
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}")]
    public IActionResult UpdateAddress(Guid id,UpdateEmployeeAddressDto dto)
    {
        service.UpdateAddress(id, dto);

        return Ok("Address Updated Successfully");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpDelete("{id}")]
    public IActionResult DeleteAddress(Guid id)
    {
        service.DeleteAddress(id);

        return Ok("Address Deleted Successfully");
    }
}
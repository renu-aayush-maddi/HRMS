using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Department;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        this.departmentService = departmentService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult GetAllDepartments()
    {
        return Ok(departmentService.GetAllDepartments());
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult AddDepartment(AddDepartmentDto dto)
    {
        departmentService.AddDepartment(dto);

        return Ok("Department Added Successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public IActionResult UpdateDepartment(Guid id, UpdateDepartmentDto dto)
    {
        departmentService.UpdateDepartment(id, dto);

        return Ok("Department Updated Successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public IActionResult DeleteDepartment(Guid id)
    {
        departmentService.DeleteDepartment(id);

        return Ok("Department Deleted Successfully");
    }
}
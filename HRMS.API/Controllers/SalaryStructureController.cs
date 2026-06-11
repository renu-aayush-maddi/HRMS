using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.SalaryStructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/salary-structures")]
[ApiController]
[Authorize]
public class SalaryStructureController
    : ControllerBase
{
    private readonly ISalaryStructureService salaryStructureService;

    public SalaryStructureController(
        ISalaryStructureService salaryStructureService)
    {
        this.salaryStructureService =
            salaryStructureService;
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost]
    public IActionResult Create(
        CreateSalaryStructureDto dto)
    {
        salaryStructureService.Create(dto);

        return Ok(
            "Salary Structure Created Successfully");
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(
            salaryStructureService.GetAll());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(
        Guid id)
    {
        return Ok(
            salaryStructureService.GetById(id));
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{id}")]
    public IActionResult Update(
        Guid id,
        UpdateSalaryStructureDto dto)
    {
        salaryStructureService.Update(
            id,
            dto);

        return Ok(
            "Salary Structure Updated Successfully");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpDelete("{id}")]
    public IActionResult Delete(
        Guid id)
    {
        salaryStructureService.Delete(id);

        return Ok(
            "Salary Structure Deleted Successfully");
    }
}
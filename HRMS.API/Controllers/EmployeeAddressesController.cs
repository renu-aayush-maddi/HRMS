using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.EmployeeAddress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;


[Route("api/employee-addresses")]
[ApiController]
[Authorize]
public class EmployeeAddressesController : ControllerBase
{
    private readonly IEmployeeAddressService service;

    public EmployeeAddressesController(IEmployeeAddressService service)
    {
        this.service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddAddress([FromBody] AddEmployeeAddressDto dto, CancellationToken cancellationToken)
    {
        var result = await service.AddAddressAsync(dto, cancellationToken);

        return CreatedAtAction(nameof(GetAddresses), new { employeeId = result.EmployeeId }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAddresses([FromQuery] EmployeeAddressFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await service.GetAddressesAsync(filter, cancellationToken);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] UpdateEmployeeAddressDto dto, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAddressAsync(id, dto, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAddress(Guid id, CancellationToken cancellationToken)
    {
        await service.DeleteAddressAsync(id, cancellationToken);

        return Ok(new ApiResponse
        {
            Message = "Address deleted successfully."
        });
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("export")]
    public async Task<IActionResult> ExportAddresses([FromQuery] EmployeeAddressFilterDto filter, CancellationToken cancellationToken)
    {
        var fileBytes = await service.ExportAddressesAsync(filter, cancellationToken);

        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"employee-addresses-{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }
    
    [Authorize(Roles = "Admin,HR")]
    [HttpPost("import")]
    public async Task<IActionResult> ImportAddresses(IFormFile file, CancellationToken cancellationToken)
    {
        var result = await service.ImportAddressesAsync(file, cancellationToken);

        return Ok(result);
    }
}
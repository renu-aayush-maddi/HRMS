using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Bonus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/bonuses")]
[ApiController]
[Authorize]
public class BonusController : ControllerBase
{
    private readonly IBonusService bonusService;

    public BonusController(IBonusService bonusService)
    {
        this.bonusService = bonusService;
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost]
    public async Task<IActionResult> CreateBonus([FromBody] CreateBonusDto dto, CancellationToken cancellationToken)
    {
        var result = await bonusService.CreateBonusAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetBonus), new { bonusId = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetBonuses([FromQuery] BonusFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await bonusService.GetBonusesAsync(filter, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{bonusId:guid}")]
    public async Task<IActionResult> GetBonus(Guid bonusId, CancellationToken cancellationToken)
    {
        var result = await bonusService.GetBonusAsync(bonusId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyBonuses([FromQuery] BonusFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await bonusService.GetMyBonusesAsync(filter, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{bonusId:guid}/approve")]
    public async Task<IActionResult> ApproveBonus(Guid bonusId, CancellationToken cancellationToken)
    {
        await bonusService.ApproveBonusAsync(bonusId, cancellationToken);
        return Ok(new { Message = "Bonus approved successfully." });
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{bonusId:guid}/reject")]
    public async Task<IActionResult> RejectBonus(Guid bonusId, CancellationToken cancellationToken)
    {
        await bonusService.RejectBonusAsync(bonusId, cancellationToken);
        return Ok(new { Message = "Bonus rejected successfully." });
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("export")]
    public async Task<IActionResult> ExportBonuses([FromQuery] BonusFilterDto filter, CancellationToken cancellationToken)
    {
        var fileBytes = await bonusService.ExportBonusesAsync(filter, cancellationToken);
        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"bonuses-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }
}
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Bonus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.API.Controllers;

[Route("api/bonuses")]
[ApiController]
[Authorize]
public class BonusController : ControllerBase
{
    private readonly IBonusService
        bonusService;

    public BonusController(
        IBonusService bonusService)
    {
        this.bonusService =
            bonusService;
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost]
    public IActionResult CreateBonus(
        CreateBonusDto dto)
    {
        bonusService.CreateBonus(dto);

        return Ok(
            "Bonus Created Successfully");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(
            bonusService
            .GetAllBonuses());
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{bonusId}/approve")]
    public IActionResult ApproveBonus(
        Guid bonusId)
    {
        bonusService
            .ApproveBonus(
                bonusId);

        return Ok(
            "Bonus Approved");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{bonusId}/reject")]
    public IActionResult RejectBonus(
        Guid bonusId)
    {
        bonusService
            .RejectBonus(
                bonusId);

        return Ok(
            "Bonus Rejected");
    }

    [HttpGet("my-bonuses")]
    public IActionResult MyBonuses()
    {
        var userId =
            Guid.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!
                    .Value);

        return Ok(
            bonusService
            .GetMyBonuses(
                userId));
    }
}
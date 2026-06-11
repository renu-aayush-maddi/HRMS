using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/users")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserManagementService service;

    public UsersController(IUserManagementService service)
    {
        this.service = service;
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        return Ok(service.GetAllUsers());
    }

    [HttpPut("{id}/disable")]
    public IActionResult Disable(Guid id)
    {
        service.DisableUser(id);

        return Ok("User disabled");
    }

    [HttpPut("{id}/activate")]
    public IActionResult Activate(Guid id)
    {
        service.ActivateUser(id);

        return Ok("User activated");
    }

    [HttpPut("{id}/reset-password")]
    public IActionResult ResetPassword(Guid id)
    {
        return Ok(service.ResetPassword(id));
    }

    [HttpPut("{id}/role")]
    public IActionResult ChangeRole(Guid id,ChangeRoleDto dto)
    {
        service.ChangeRole(id, dto);

        return Ok("Role updated");
    }


    [HttpGet("{id}")]
    public IActionResult GetUser(Guid id)
    {
        var user = service.GetUser(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }
}
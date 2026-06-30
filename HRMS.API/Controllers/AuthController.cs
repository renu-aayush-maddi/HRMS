using HRMS.API.Models.DTOs.Auth;
using HRMS.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace HRMS.API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService  _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.Register(dto);
        return Ok(result);

    }


    [AllowAnonymous]
    [EnableRateLimiting("LoginPolicy")]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.Login(dto);
        return Ok(result);
    }
    [HttpGet("test-token")]
    [Authorize]
    public IActionResult TestToken()
    {
        return Ok(new
        {
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Email = User.FindFirst(ClaimTypes.Email)?.Value,
            Role = User.FindFirst(ClaimTypes.Role)?.Value
        });
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var origin = Request.Headers["Origin"].ToString();
        if (string.IsNullOrEmpty(origin))
        {
            origin = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(origin))
            {
                var uri = new Uri(origin);
                origin = $"{uri.Scheme}://{uri.Authority}";
            }
            else
            {
                origin = "http://localhost:4200";
            }
        }
        else
        {
            origin = origin.TrimEnd('/');
        }

        await _authService.ForgotPassword(dto, origin);
        return Ok(new { Message = "If an account exists with this email, a password reset link has been sent." });
    }

    [AllowAnonymous]
    [HttpGet("validate-reset-token")]
    public async Task<IActionResult> ValidateResetToken([FromQuery] string token)
    {
        var isValid = await _authService.ValidateResetToken(token);
        return Ok(new { Valid = isValid });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        await _authService.ResetPassword(dto);
        return Ok(new { Message = "Password has been reset successfully." });
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized();
        }

        await _authService.ChangePassword(userId, dto);
        return Ok(new { Message = "Password has been changed successfully." });
    }
}
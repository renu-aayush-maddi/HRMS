namespace HRMS.API.Models.DTOs.Auth;

public class LoginResponseDto
{
    public string Token { get; set; }

    public string Email { get; set; }

    public string Role { get; set; }
}
namespace HRMS.API.Models.DTOs.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public Guid UserId { get; set; }

    public Guid? EmployeeId { get; set; }
}
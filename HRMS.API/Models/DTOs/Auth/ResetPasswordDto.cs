using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models.DTOs.Auth;

public class ResetPasswordDto
{
    [Required]
    public string Token { get; set; } = null!;

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string NewPassword { get; set; } = null!;
}

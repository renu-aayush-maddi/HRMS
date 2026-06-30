using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models.DTOs.Auth;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
}

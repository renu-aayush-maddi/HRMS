namespace HRMS.API.Models.DTOs.Auth;
using System.ComponentModel.DataAnnotations;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(8)]
    public string Password { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; } 

    [Required]
    public string Role { get; set; }

    [Required]
    public Guid DepartmentId { get; set; }

    [Required]
    public string Designation { get; set; } 

    [Required]
    [Range(1, 10000000, ErrorMessage = "Salary must be greater than zero")]
    public decimal Salary { get; set; }

}
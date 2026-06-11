namespace HRMS.API.Models.DTOs.User;

public class UserResponseDto
{
    public Guid Id { get; set; }

    public string Email { get; set; }

    public string Role { get; set; }

    public bool IsActive { get; set; }

    public string EmployeeName { get; set; }
}
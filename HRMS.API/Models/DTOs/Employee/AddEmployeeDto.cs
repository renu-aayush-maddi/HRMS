using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models.DTOs.Employee;

public class AddEmployeeDto
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Phone]
    public string? Phone { get; set; }

    [Required]
    public string? Designation { get; set; }

    [Required]
    public Guid DepartmentId { get; set; }

    [Range(1, 10000000)]
    public decimal Salary { get; set; }

    [Required]
    public string Role { get; set; } = string.Empty;

    public Guid? ManagerId { get; set; }
}
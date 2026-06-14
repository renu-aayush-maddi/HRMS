using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models.DTOs.Employee;

public class UpdateEmployeeStatusDto
{
    [Required]
    public string Status { get; set; } = string.Empty;
}
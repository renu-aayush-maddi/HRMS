using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models.DTOs.Department;

public class AddDepartmentDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
}
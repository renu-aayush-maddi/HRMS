namespace HRMS.API.Models.DTOs.Holiday;
using System.ComponentModel.DataAnnotations;

public class UpdateHolidayDto
{

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateOnly HolidayDate { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public bool IsOptional { get; set; }
}
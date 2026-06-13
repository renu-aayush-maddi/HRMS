using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models.DTOs.PerformanceCycle;

public class AddPerformanceCycleDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }
        = string.Empty;

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }
}
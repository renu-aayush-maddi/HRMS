using System.ComponentModel.DataAnnotations;
using HRMS.API.Validations;

public class AddGoalDto
{
    [ValidGuid(ErrorMessage = "EmployeeId is required")]
    public Guid EmployeeId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; }
        = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; }
        = string.Empty;

    [Required]
    public DateOnly? TargetDate { get; set; }
}
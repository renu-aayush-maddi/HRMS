namespace HRMS.API.Models.DTOs.Goal;

public class GoalResponseDto
{
    public Guid Id { get; set; }

    public string EmployeeName { get; set; }
        = string.Empty;

    public string Title { get; set; }
        = string.Empty;

    public string Description { get; set; }
        = string.Empty;

    public DateOnly? TargetDate { get; set; }

    public string Status { get; set; }
        = string.Empty;
}
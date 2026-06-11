namespace HRMS.API.Models.DTOs.EmployeeExperience;

public class EmployeeExperienceResponseDto
{
    public Guid Id { get; set; }

    public string CompanyName { get; set; }
        = string.Empty;

    public string Designation { get; set; }
        = string.Empty;

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Responsibilities { get; set; }
}
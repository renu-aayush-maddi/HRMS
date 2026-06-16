namespace HRMS.API.Models.DTOs.EmployeeExperience;

public class EmployeeExperienceImportResultDto
{
    public int TotalRows { get; set; }

    public int SuccessCount { get; set; }

    public int FailedCount { get; set; }

    public List<string> Errors { get; set; } = [];
}
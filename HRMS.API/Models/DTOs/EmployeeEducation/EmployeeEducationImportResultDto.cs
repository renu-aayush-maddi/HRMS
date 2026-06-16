namespace HRMS.API.Models.DTOs.EmployeeEducation;

public class EmployeeEducationImportResultDto
{
    public int TotalRows { get; set; }

    public int SuccessCount { get; set; }

    public int FailedCount { get; set; }

    public List<string> Errors { get; set; } = [];
}
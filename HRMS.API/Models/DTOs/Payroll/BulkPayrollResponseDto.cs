namespace HRMS.API.Models.DTOs.Payroll;

public class BulkPayrollResponseDto
{
    public int TotalEmployees { get; set; }

    public int SuccessCount { get; set; }

    public int FailedCount { get; set; }

    public int SkippedCount { get; set; }

    public List<string> Errors { get; set; } = [];

    public List<string> SkippedEmployees { get; set; } = [];
}
namespace HRMS.API.Models.DTOs.EmployeeAddress;

public class EmployeeAddressImportResultDto
{
    public int TotalRows { get; set; }

    public int SuccessCount { get; set; }

    public int FailedCount { get; set; }

    public List<string> Errors { get; set; } = [];
}
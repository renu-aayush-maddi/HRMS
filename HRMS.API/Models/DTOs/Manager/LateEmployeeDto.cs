namespace HRMS.API.Models.DTOs.Manager;

public class LateEmployeeDto
{
    public string EmployeeName { get; set; }
        = string.Empty;

    public DateTime? CheckIn { get; set; }
}
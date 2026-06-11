namespace HRMS.API.Models.DTOs.Dashboard;


public class DepartmentSummaryDto
{
    public string DepartmentName { get; set; }

    public int EmployeeCount { get; set; }

    public int ActiveEmployees { get; set; }

    public int OnLeaveEmployees { get; set; }
}
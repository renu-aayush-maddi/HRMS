namespace HRMS.API.Models.DTOs.Dashboard;

public class EmployeePerformanceDto
{
    public Guid EmployeeId { get; set; }

    public string EmployeeName { get; set; }
        = string.Empty;

    public decimal AverageRating { get; set; }
}
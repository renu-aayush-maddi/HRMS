namespace HRMS.API.Models.DTOs.Manager;

public class TeamAttendanceDto
{
    public string EmployeeName { get; set; }
        = string.Empty;

    public DateOnly AttendanceDate { get; set; }

    public DateTime? CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    public string? Status { get; set; }
}
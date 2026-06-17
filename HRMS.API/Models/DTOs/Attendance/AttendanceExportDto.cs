namespace HRMS.API.Models.DTOs.Attendance;

public class AttendanceExportDto
{
    public string EmployeeCode { get; set; }
        = string.Empty;

    public string EmployeeName { get; set; }
        = string.Empty;

    public DateOnly AttendanceDate { get; set; }

    public DateTime? CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    public decimal? WorkingHours { get; set; }

    public string Status { get; set; }
        = string.Empty;
}
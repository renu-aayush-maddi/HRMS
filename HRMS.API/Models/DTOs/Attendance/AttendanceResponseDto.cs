namespace HRMS.API.Models.DTOs.Attendance;

public class AttendanceResponseDto
{
    public Guid Id { get; set; }

    public string EmployeeName { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public DateTime? CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    public string? Status { get; set; }
}
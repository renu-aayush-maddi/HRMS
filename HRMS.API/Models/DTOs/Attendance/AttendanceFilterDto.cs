namespace HRMS.API.Models.DTOs.Attendance;

public class AttendanceFilterDto
{
    public Guid? EmployeeId { get; set; }

    public string? Status { get; set; }

    public DateOnly? FromDate { get; set; }

    public DateOnly? ToDate { get; set; }

    public string? SortBy { get; set; }

    public string? SortDirection { get; set; }
}
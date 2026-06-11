namespace HRMS.API.Models.DTOs.Attendance;

public class AttendanceQueryDto
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public Guid? EmployeeId { get; set; }

    public string? Status { get; set; }

    public DateOnly? FromDate { get; set; }

    public DateOnly? ToDate { get; set; }

    public string? SortBy { get; set; }

    public string? SortDirection { get; set; }
}
namespace HRMS.API.Models.DTOs.AttendanceRegularization;

public class AttendanceRegularizationResponseDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string EmployeeCode { get; set; }
        = string.Empty;

    public string EmployeeName { get; set; }
        = string.Empty;

    public DateOnly AttendanceDate { get; set; }

    public DateTime? RequestedCheckIn { get; set; }

    public DateTime? RequestedCheckOut { get; set; }

    public string Reason { get; set; }
        = string.Empty;

    public string Status { get; set; }
        = string.Empty;

    public string? HrComments { get; set; }

    public Guid? ReviewedBy { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public DateTime? CreatedAt { get; set; }
}
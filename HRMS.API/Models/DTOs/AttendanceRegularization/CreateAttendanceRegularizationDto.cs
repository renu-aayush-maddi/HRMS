namespace HRMS.API.Models.DTOs.AttendanceRegularization;

public class CreateAttendanceRegularizationDto
{
    public Guid? EmployeeId { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public DateTime? RequestedCheckIn { get; set; }

    public DateTime? RequestedCheckOut { get; set; }

    public string Reason { get; set; } = string.Empty;
}
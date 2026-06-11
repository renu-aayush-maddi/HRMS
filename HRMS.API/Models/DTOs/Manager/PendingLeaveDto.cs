namespace HRMS.API.Models.DTOs.Manager;

public class PendingLeaveDto
{
    public Guid LeaveId { get; set; }

    public string EmployeeName { get; set; }
        = string.Empty;

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }
}
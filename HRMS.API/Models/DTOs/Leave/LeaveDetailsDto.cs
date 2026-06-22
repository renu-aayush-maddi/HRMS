namespace HRMS.API.Models.DTOs.Leave;

public class LeaveDetailsDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string EmployeeName { get; set; } = string.Empty;

    public Guid LeaveTypeId { get; set; }

    public string LeaveType { get; set; } = string.Empty;

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public int TotalDays { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public string? ManagerComments { get; set; }

    public DateTime? CreatedAt { get; set; }
}
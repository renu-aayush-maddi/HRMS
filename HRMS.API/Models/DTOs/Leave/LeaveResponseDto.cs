namespace HRMS.API.Models.DTOs.Leave;

public class LeaveResponseDto
{
    public Guid Id { get; set; }

    public string EmployeeName { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public string? ManagerComments { get; set; }

    public string LeaveType { get; set; }
}
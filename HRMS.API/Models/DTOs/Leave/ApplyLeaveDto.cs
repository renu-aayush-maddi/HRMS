namespace HRMS.API.Models.DTOs.Leave;

public class ApplyLeaveDto
{
    public Guid? EmployeeId { get; set; }

     public Guid LeaveTypeId { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public string? Reason { get; set; }
}
namespace HRMS.API.Models.DTOs.Leave;

public class LeaveBalanceDto
{
    public Guid LeaveTypeId { get; set; }

    public string LeaveTypeName { get; set; } = string.Empty;

    public int AllocatedDays { get; set; }

    public int UsedDays { get; set; }

    public int RemainingDays { get; set; }
}
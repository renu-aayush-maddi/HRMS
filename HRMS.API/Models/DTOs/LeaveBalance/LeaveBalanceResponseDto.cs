namespace HRMS.API.Models.DTOs.LeaveBalance;

public class LeaveBalanceResponseDto
{
    public Guid Id { get; set; }

    public string EmployeeName { get; set; }

    public string LeaveType { get; set; }

    public int AllocatedDays { get; set; }

    public int UsedDays { get; set; }

    public int RemainingDays { get; set; }
}
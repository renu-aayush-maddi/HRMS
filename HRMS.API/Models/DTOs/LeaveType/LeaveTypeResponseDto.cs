namespace HRMS.API.Models.DTOs.LeaveType;

public class LeaveTypeResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public int AnnualAllocation { get; set; }

    public bool CarryForwardAllowed { get; set; }

    public int MaxCarryForward { get; set; }

    public bool NegativeBalanceAllowed { get; set; }

    public bool IsActive { get; set; }
}
namespace HRMS.API.Models.DTOs.LeaveType;

public class UpdateLeaveTypeDto
{
    public string Name { get; set; }

    public int AnnualAllocation { get; set; }

    public bool CarryForwardAllowed { get; set; }

    public int MaxCarryForward { get; set; }

    public bool NegativeBalanceAllowed { get; set; }

    public bool IsActive { get; set; }
}
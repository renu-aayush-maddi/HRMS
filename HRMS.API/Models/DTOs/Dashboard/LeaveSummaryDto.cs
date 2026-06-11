namespace HRMS.API.Models.DTOs.Dashboard;

public class LeaveSummaryDto
{
    public int PendingLeaves { get; set; }

    public int ApprovedLeaves { get; set; }

    public int RejectedLeaves { get; set; }

    public int TotalLeaves { get; set; }
}
namespace HRMS.API.Models.DTOs.Manager;

public class ManagerDashboardDto
{
    public int TeamSize { get; set; }

    public int PresentToday { get; set; }

    public int OnLeaveToday { get; set; }

    public int PendingLeaveRequests { get; set; }

    public int PendingRegularizations { get; set; }
}
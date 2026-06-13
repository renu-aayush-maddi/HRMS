namespace HRMS.API.Models.DTOs.Dashboard;

public class ManagerDashboardDto
{
    public int TeamMembers { get; set; }

    public int TotalGoals { get; set; }

    public int CompletedGoals { get; set; }

    public int PendingGoals { get; set; }

    public decimal AverageRating { get; set; }

    public string TopPerformer { get; set; }
        = string.Empty;
}
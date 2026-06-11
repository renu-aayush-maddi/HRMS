namespace HRMS.API.Models.DTOs.Dashboard;

public class HrDashboardStatsDto
{
    public int TotalEmployees { get; set; }

    public int ActiveEmployees { get; set; }

    public int OnLeaveEmployees { get; set; }

    public int ProbationEmployees { get; set; }

    public int Departments { get; set; }

    public int PendingLeaves { get; set; }

    public int ApprovedLeaves { get; set; }

    public int PayrollGeneratedThisMonth { get; set; }

    public decimal TotalMonthlyPayroll { get; set; }

    public int PresentToday { get; set; }

    public int AbsentToday { get; set; }

    public int LateEmployees { get; set; }

    public decimal AttendancePercentage { get; set; }

    public int NewHiresThisMonth { get; set; }
}
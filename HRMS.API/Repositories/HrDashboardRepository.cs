using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Dashboard;


namespace HRMS.API.Repositories;

public class HrDashboardRepository
    : IHrDashboardRepository
{
    private readonly AppDbContext context;

    public HrDashboardRepository(AppDbContext context)
    {
        this.context = context;
    }

    public List<DepartmentSummaryDto> GetDepartmentSummary()
        {
            return context.Departments
                .Select(d =>
                    new DepartmentSummaryDto
                    {
                        DepartmentName = d.Name,

                        EmployeeCount =
                            d.Employees.Count(),

                        ActiveEmployees =
                            d.Employees.Count(e =>
                                e.EmploymentStatus == "Active"),

                        OnLeaveEmployees =
                            d.Employees.Count(e =>
                                e.EmploymentStatus == "OnLeave")
                    })
                .ToList();
        }

    public LeaveSummaryDto GetLeaveSummary()
    {
        return new LeaveSummaryDto
        {
            PendingLeaves =
                context.LeaveRequests
                .Count(x => x.Status == "Pending"),

            ApprovedLeaves =
                context.LeaveRequests
                .Count(x => x.Status == "Approved"),

            RejectedLeaves =
                context.LeaveRequests
                .Count(x => x.Status == "Rejected"),

            TotalLeaves =
                context.LeaveRequests.Count()
        };
    }


    public PayrollSummaryDto GetPayrollSummary()
        {
            int currentMonth =
                DateTime.Now.Month;

            int currentYear =
                DateTime.Now.Year;

            var payrolls =
                context.Payrolls
                .Where(p =>
                    p.PayMonth == currentMonth &&
                    p.PayYear == currentYear);

            return new PayrollSummaryDto
            {
                TotalPayroll =
                    payrolls.Sum(x =>
                        x.NetSalary ?? 0),

                AverageSalary =
                    payrolls.Any()
                    ? payrolls.Average(x =>
                        x.NetSalary ?? 0)
                    : 0,

                HighestSalary =
                    payrolls.Any()
                    ? payrolls.Max(x =>
                        x.NetSalary ?? 0)
                    : 0,

                EmployeesPaid =
                    payrolls.Count()
            };
        }

    public HrDashboardStatsDto GetStats()
    {
        int currentMonth = DateTime.Now.Month;
        int currentYear = DateTime.Now.Year;
        var today = DateOnly.FromDateTime(DateTime.Now);
        int totalEmployees = context.Employees.Count();

        int presentToday = context.AttendanceLogs.Count(a =>a.AttendanceDate == today);

        int onLeaveToday =
            context.LeaveRequests
                .Count(l =>
                    l.Status == "Approved"
                    &&
                    l.FromDate <= today
                    &&
                    l.ToDate >= today);

        int absentToday = totalEmployees - presentToday - onLeaveToday;

        int lateEmployees =
            context.AttendanceLogs
                .Count(a =>
                    a.AttendanceDate == today
                    &&
                    a.CheckIn != null
                    &&
                    a.CheckIn.Value.TimeOfDay >
                    new TimeSpan(9, 30, 0));

        decimal attendancePercentage = totalEmployees == 0? 0: Math.Round((decimal)presentToday/ totalEmployees * 100,2);

        int newHiresThisMonth =
    context.Employees.Count(e =>
        e.JoiningDate.Month == currentMonth &&
        e.JoiningDate.Year == currentYear);

        return new HrDashboardStatsDto
        {
            TotalEmployees =
                totalEmployees,

            ActiveEmployees =
                context.Employees
                .Count(e =>
                    e.EmploymentStatus == "Active"),

            OnLeaveEmployees =
                context.Employees
                .Count(e =>
                    e.EmploymentStatus == "OnLeave"),

            ProbationEmployees =
                context.Employees
                .Count(e =>
                    e.EmploymentStatus == "Probation"),

            Departments = context.Departments.Count(),

            PendingLeaves =
                context.LeaveRequests
                .Count(l =>
                    l.Status == "Pending"),

            ApprovedLeaves =
                context.LeaveRequests
                .Count(l =>
                    l.Status == "Approved"),

            PresentToday = presentToday,

            AbsentToday = absentToday,

            LateEmployees = lateEmployees,

            AttendancePercentage = attendancePercentage,

            PayrollGeneratedThisMonth =
                context.Payrolls
                .Count(p =>
                    p.PayMonth == currentMonth &&
                    p.PayYear == currentYear),

            TotalMonthlyPayroll =
                context.Payrolls
                .Where(p =>
                    p.PayMonth == currentMonth &&
                    p.PayYear == currentYear)
                .Sum(p =>
                    p.NetSalary ?? 0),

            NewHiresThisMonth = newHiresThisMonth
        };

    }
}
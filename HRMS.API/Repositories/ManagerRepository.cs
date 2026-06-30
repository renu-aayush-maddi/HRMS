using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Manager;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace HRMS.API.Repositories;

public class ManagerRepository: IManagerRepository
{
    private readonly AppDbContext context;

    public ManagerRepository(
        AppDbContext context)
    {
        this.context = context;
    }

    public Employee? GetEmployeeByUserId(
        Guid userId)
    {
        return context.Employees
            .FirstOrDefault(e =>
                e.UserId == userId);
    }

    public ManagerDashboardDto GetDashboard(
        Guid managerEmployeeId)
    {
        var today =
            DateOnly.FromDateTime(
                DateTime.Today);

        var teamIds =
            context.Employees
            .Where(e =>
                e.ManagerId ==
                managerEmployeeId)
            .Select(e =>
                e.Id)
            .ToList();

        return new ManagerDashboardDto
        {
            TeamSize =
                teamIds.Count,

            PresentToday =
                context.AttendanceLogs
                .Count(a =>
                    teamIds.Contains(
                        a.EmployeeId!.Value)
                    &&
                    a.AttendanceDate ==
                    today),

            OnLeaveToday =
                context.LeaveRequests
                .Count(l =>
                    teamIds.Contains(
                        l.EmployeeId!.Value)
                    &&
                    l.Status == "Approved"
                    &&
                    l.FromDate <= today
                    &&
                    l.ToDate >= today),

            PendingLeaveRequests =
                context.LeaveRequests
                .Count(l =>
                    teamIds.Contains(
                        l.EmployeeId!.Value)
                    &&
                    l.Status == "Pending"),

            PendingRegularizations =
                context
                .AttendanceRegularizations
                .Count(r =>
                    teamIds.Contains(
                        r.EmployeeId)
                    &&
                    r.Status == "Pending")
        };
    }

    public List<Employee> GetTeamMembers(
        Guid managerEmployeeId)
    {
        return context.Employees
            .Include(e => e.Department)
            .Where(e =>
                e.ManagerId ==
                managerEmployeeId)
            .ToList();
    }


    public Employee? GetTeamMember(
        Guid managerEmployeeId,
        Guid employeeId)
    {
        return context.Employees
            .Include(e => e.Department)
            .FirstOrDefault(e =>
                e.Id == employeeId &&
                e.ManagerId == managerEmployeeId);
    }


    public List<AttendanceLog> GetTeamAttendance(
    Guid managerEmployeeId)
    {
        return context.AttendanceLogs
            .Include(a => a.Employee)
            .Where(a =>
                a.Employee!.ManagerId ==
                managerEmployeeId)
            .OrderByDescending(a =>
                a.AttendanceDate)
            .ToList();
    }

    public List<AttendanceLog>
    GetLateEmployees(
        Guid managerEmployeeId)
    {
        var today =
            DateOnly.FromDateTime(
                DateTime.Today);

        return context.AttendanceLogs
            .Include(a => a.Employee)
            .Where(a =>
                a.Employee!.ManagerId ==
                managerEmployeeId
                &&
                a.AttendanceDate ==
                today
                &&
                a.CheckIn != null
                &&
                a.CheckIn.Value.TimeOfDay >
                new TimeSpan(9,30,0))
            .ToList();
    }

    public List<LeaveRequest>
    GetPendingLeaveRequests(
        Guid managerEmployeeId)
{
    return context.LeaveRequests
        .Include(l => l.Employee)
        .Where(l =>
            l.Employee!.ManagerId ==
            managerEmployeeId
            &&
            l.Status == "Pending")
        .OrderBy(l =>
            l.FromDate)
        .ToList();
}


    public List<PerformanceReview> GetPerformanceReviews(Guid managerEmployeeId)
    {
        return context.PerformanceReviews
            .Include(p => p.Employee)
            .Where(p =>
                p.Employee!.ManagerId ==
                managerEmployeeId)
            .OrderByDescending(p =>
                p.ReviewDate)
            .ToList();
    }


    public List<PerformanceReview>
        GetEmployeePerformanceReviews(
            Guid managerEmployeeId,
            Guid employeeId)
    {
        return context.PerformanceReviews
            .Include(p => p.Employee)
            .Where(p =>
                p.EmployeeId == employeeId
                &&
                p.Employee!.ManagerId ==
                managerEmployeeId)
            .ToList();
    }


    public void AddPerformanceReview(
        PerformanceReview review)
    {
        context.PerformanceReviews
            .Add(review);
    }

    public Employee? GetEmployeeById(Guid id)
    {
        return context.Employees
            .Include(e => e.User)
            .ThenInclude(u => u!.Roles)
            .FirstOrDefault(e => e.Id == id);
    }

    public (List<Employee> Employees, int TotalCount) GetEligibleEmployees(Guid managerEmployeeId, string? search, int page, int pageSize)
    {
        var query = context.Employees
            .Include(e => e.Department)
            .Include(e => e.Manager)
            .Include(e => e.User)
            .ThenInclude(u => u!.Roles)
            .Where(e => !e.IsDeleted 
                     && e.EmploymentStatus == "Active"
                     && e.Id != managerEmployeeId);

        // Exclude users with Admin, HR, or Manager roles
        query = query.Where(e => e.User != null && !e.User.Roles.Any(r => r.Name == "Admin" || r.Name == "HR" || r.Name == "Manager"));

        if (!string.IsNullOrWhiteSpace(search))
        {
            var cleanSearch = search.Trim().ToLower();
            query = query.Where(e => e.FirstName.ToLower().Contains(cleanSearch) 
                                  || e.LastName.ToLower().Contains(cleanSearch) 
                                  || e.EmployeeCode.ToLower().Contains(cleanSearch)
                                  || e.Email.ToLower().Contains(cleanSearch));
        }

        var totalCount = query.Count();

        var employees = query
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (employees, totalCount);
    }

    public PerformanceCycle? GetPerformanceCycle(Guid id)
    {
        return context.PerformanceCycles.Find(id);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}
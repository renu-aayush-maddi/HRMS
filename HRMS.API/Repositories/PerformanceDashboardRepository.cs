using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using HRMS.API.Models.DTOs.Dashboard;

namespace HRMS.API.Repositories;

public class PerformanceDashboardRepository
    : IPerformanceDashboardRepository
{
    private readonly AppDbContext context;

    public PerformanceDashboardRepository(
        AppDbContext context)
    {
        this.context = context;
    }

    public Employee? GetEmployeeByUserId(
        Guid userId)
    {
        return context.Employees
            .FirstOrDefault(
                e => e.UserId == userId);
    }

    public int GetTeamMembersCount(
        Guid managerId)
    {
        return context.Employees
            .Count(
                e => e.ManagerId == managerId);
    }

    public int GetTotalGoals(
        Guid managerId)
    {
        return context.EmployeeGoals
            .Include(g => g.Employee)
            .Count(
                g => g.Employee!.ManagerId ==
                     managerId);
    }

    public int GetCompletedGoals(
        Guid managerId)
    {
        return context.EmployeeGoals
            .Include(g => g.Employee)
            .Count(
                g => g.Employee!.ManagerId ==
                     managerId
                     &&
                     g.Status == "Completed");
    }

    public decimal GetAverageRating(
        Guid managerId)
    {
        var ratings =
            context.PerformanceReviews
            .Include(r => r.Employee)
            .Where(
                r => r.Employee!.ManagerId ==
                     managerId);

        if (!ratings.Any())
        {
            return 0;
        }

        return Math.Round(
            ratings.Average(
                r => r.Rating ?? 0),
            2);
    }

    public string? GetTopPerformer(
        Guid managerId)
    {
        return context.PerformanceReviews
            .Include(r => r.Employee)
            .Where(
                r => r.Employee!.ManagerId ==
                     managerId)
            .GroupBy(
                r => new
                {
                    r.EmployeeId,
                    Name =
                        r.Employee!.FirstName +
                        " " +
                        r.Employee.LastName
                })
            .Select(
                g => new
                {
                    g.Key.Name,

                    AverageRating =
                        g.Average(
                            x => x.Rating ?? 0)
                })
            .OrderByDescending(
                x => x.AverageRating)
            .Select(
                x => x.Name)
            .FirstOrDefault();
    }
    public int GetTotalEmployees()
    {
        return context.Employees.Count();
    }

    public int GetTotalReviews()
    {
        return context.PerformanceReviews.Count();
    }

    public decimal GetCompanyAverageRating()
    {
        if (!context.PerformanceReviews.Any())
        {
            return 0;
        }

        return Math.Round(
            context.PerformanceReviews
                .Average(r => r.Rating ?? 0),
            2);
    }


    public decimal GetReviewCompletionPercentage()
    {
        var totalEmployees =
            context.Employees.Count();

        if (totalEmployees == 0)
        {
            return 0;
        }

        var reviewedEmployees =
            context.PerformanceReviews
                .Select(r => r.EmployeeId)
                .Distinct()
                .Count();

        return Math.Round(
            reviewedEmployees * 100m /
            totalEmployees,
            2);
    }

    public List<EmployeePerformanceDto>
    GetTopPerformers()
    {
        return context.PerformanceReviews
            .Include(r => r.Employee)
            .GroupBy(r => new
            {
                r.EmployeeId,
                Name =
                    r.Employee!.FirstName +
                    " " +
                    r.Employee.LastName
            })
            .Select(g =>
                new EmployeePerformanceDto
                {
                    EmployeeId =
                        g.Key.EmployeeId!.Value,

                    EmployeeName =
                        g.Key.Name,

                    AverageRating =
                        Math.Round(
                            g.Average(
                                x => x.Rating ?? 0),
                            2)
                })
            .OrderByDescending(
                x => x.AverageRating)
            .Take(10)
            .ToList();
    }

    public List<EmployeePerformanceDto>
    GetLowestPerformers()
    {
        return context.PerformanceReviews
            .Include(r => r.Employee)
            .GroupBy(r => new
            {
                r.EmployeeId,
                Name =
                    r.Employee!.FirstName +
                    " " +
                    r.Employee.LastName
            })
            .Select(g =>
                new EmployeePerformanceDto
                {
                    EmployeeId =
                        g.Key.EmployeeId!.Value,

                    EmployeeName =
                        g.Key.Name,

                    AverageRating =
                        Math.Round(
                            g.Average(
                                x => x.Rating ?? 0),
                            2)
                })
            .OrderBy(
                x => x.AverageRating)
            .Take(10)
            .ToList();
    }

    public List<DepartmentRatingDto>
    GetDepartmentRatings()
    {
        return context.PerformanceReviews
            .Include(r => r.Employee)
            .ThenInclude(e => e.Department)
            .GroupBy(r =>
                r.Employee!.Department!.Name)
            .Select(g =>
                new DepartmentRatingDto
                {
                    DepartmentName =
                        g.Key,

                    AverageRating =
                        Math.Round(
                            g.Average(
                                x => x.Rating ?? 0),
                            2)
                })
            .ToList();
    }
}
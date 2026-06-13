using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using HRMS.API.Models.DTOs.Goal;
using HRMS.API.Models.DTOs.Common;

namespace HRMS.API.Repositories;

public class GoalRepository : IGoalRepository
{
    private readonly AppDbContext context;

    public GoalRepository(AppDbContext context)
    {
        this.context = context;
    }

    public Employee? GetEmployeeByUserId(Guid userId)
    {
        return context.Employees.FirstOrDefault(e => e.UserId == userId);
    }

    public Employee? GetTeamMember(Guid managerId,Guid employeeId)
    {
        return context.Employees
            .FirstOrDefault(e =>
                e.Id == employeeId
                &&
                e.ManagerId == managerId);
    }

    public EmployeeGoal? GetGoal(Guid goalId)
    {
        return context.EmployeeGoals
            .Include(g => g.Employee)
            .FirstOrDefault(g => g.Id == goalId);
    }

    public List<EmployeeGoal> GetGoals(Guid managerId,GoalQueryDto query,int skip,int take)
    {
        var goals =
            context.EmployeeGoals
            .Include(g => g.Employee)
            .Where(g =>
                g.Employee!.ManagerId ==
                managerId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(
            query.Status))
        {
            goals =
                goals.Where(g =>
                    g.Status ==
                    query.Status);
        }

        if (query.FromDate.HasValue)
        {
            goals =
                goals.Where(g =>
                    g.TargetDate >=
                    query.FromDate.Value);
        }

        if (query.ToDate.HasValue)
        {
            goals =
                goals.Where(g =>
                    g.TargetDate <=
                    query.ToDate.Value);
        }

        goals =
            ApplySorting(
                goals,
                query);

        return goals
            .Skip(skip)
            .Take(take)
            .ToList();
    }



    public int GetGoalsCount(Guid managerId,GoalQueryDto query)
    {
        var goals =
            context.EmployeeGoals
            .Where(g =>
                g.Employee!.ManagerId ==
                managerId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(
            query.Status))
        {
            goals =
                goals.Where(g =>
                    g.Status ==
                    query.Status);
        }

        if (query.FromDate.HasValue)
        {
            goals =
                goals.Where(g =>
                    g.TargetDate >=
                    query.FromDate.Value);
        }

        if (query.ToDate.HasValue)
        {
            goals =
                goals.Where(g =>
                    g.TargetDate <=
                    query.ToDate.Value);
        }

        return goals.Count();
    }


    private IQueryable<EmployeeGoal> ApplySorting(IQueryable<EmployeeGoal> goals,GoalQueryDto query)
    {
        var sortBy =
            query.SortBy?.ToLower();

        var direction =
            query.SortDirection?.ToLower();

        return (sortBy, direction) switch
        {
            ("targetdate", "desc")
                => goals.OrderByDescending(
                    g => g.TargetDate),

            ("targetdate", _)
                => goals.OrderBy(
                    g => g.TargetDate),

            ("status", "desc")
                => goals.OrderByDescending(
                    g => g.Status),

            ("status", _)
                => goals.OrderBy(
                    g => g.Status),

            ("createdat", "desc")
                => goals.OrderByDescending(
                    g => g.CreatedAt),

            ("createdat", _)
                => goals.OrderBy(
                    g => g.CreatedAt),

            _
                => goals.OrderByDescending(
                    g => g.CreatedAt)
        };
    }

    public List<EmployeeGoal> GetEmployeeGoals(Guid managerId,Guid employeeId)
    {
        return context.EmployeeGoals
            .Include(g => g.Employee)
            .Where(g =>
                g.EmployeeId == employeeId
                &&
                g.Employee!.ManagerId == managerId)
            .ToList();
    }

    public void AddGoal(EmployeeGoal goal)
    {
        context.EmployeeGoals.Add(goal);
    }

    public void UpdateGoal(EmployeeGoal goal)
    {
        context.EmployeeGoals.Update(goal);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }



    public List<EmployeeGoal> GetGoalsByEmployee(
        Guid employeeId)
    {
        return context.EmployeeGoals
            .Include(g => g.Employee)
            .Where(g =>
                g.EmployeeId == employeeId)
            .ToList();
    }

    public EmployeeGoal? GetEmployeeGoal(
        Guid goalId,
        Guid employeeId)
    {
        return context.EmployeeGoals
            .Include(g => g.Employee)
            .FirstOrDefault(g =>
                g.Id == goalId
                &&
                g.EmployeeId == employeeId);
    }
}
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Goal;
using HRMS.API.Models.Entities;
using HRMS.API.Exceptions;
using HRMS.API.Models.DTOs.Common;

namespace HRMS.API.Services;

public class GoalService : IGoalService
{
    private readonly IGoalRepository repository;

    public GoalService(IGoalRepository repository)
    {
        this.repository = repository;
    }

    public void AddGoal(Guid managerUserId,AddGoalDto dto)
    {
        var manager =repository.GetEmployeeByUserId(managerUserId);

        if (manager == null)
        {
            throw new NotFoundException("Manager not found");
        }

        var employee = repository.GetTeamMember(manager.Id,dto.EmployeeId);

        if (employee == null)
        {
            throw new BusinessException("Employee does not belong to your team");
        }

        if(dto.TargetDate <DateOnly.FromDateTime(DateTime.Today))
        {
            throw new BusinessException("Target date cannot be in the past");
        }

        EmployeeGoal goal = new()
        {
            Id = Guid.NewGuid(),

            EmployeeId = dto.EmployeeId,

            AssignedBy = manager.Id,

            Title = dto.Title,

            Description = dto.Description,

            TargetDate = dto.TargetDate,

            Status = "Assigned"
        };

        repository.AddGoal(goal);

        repository.SaveChanges();
    }

    public PaginatedResponseDto<GoalResponseDto> GetGoals(Guid managerUserId,GoalQueryDto query)
    {
        var manager =repository.GetEmployeeByUserId(managerUserId);

        if (manager == null)
        {
            throw new NotFoundException("Manager not found");
        }

        var page = query.Page <= 0? 1: query.Page;

        var pageSize = query.PageSize <= 0? 10: query.PageSize;

        var skip = (page - 1) * pageSize;

        var goals = repository.GetGoals(
                manager.Id,
                query,
                skip,
                pageSize);

        var totalRecords = repository.GetGoalsCount(manager.Id,query);

        var data =
            goals.Select(g =>
                new GoalResponseDto
                {
                    Id = g.Id,

                    EmployeeName =
                        g.Employee!.FirstName +
                        " " +
                        g.Employee.LastName,

                    Title = g.Title,

                    Description = g.Description ?? "",

                    TargetDate = g.TargetDate,

                    Status = g.Status ?? ""
                })
            .ToList();

        return new PaginatedResponseDto<GoalResponseDto>
        {
            Page = page,

            PageSize = pageSize,

            TotalRecords = totalRecords,

            Data = data
        };
    }

    public List<GoalResponseDto> GetEmployeeGoals( Guid managerUserId,Guid employeeId)
    {
        var manager = repository.GetEmployeeByUserId(managerUserId);

        if (manager == null)
        {
            throw new NotFoundException("Manager not found");
        }

        return repository
            .GetEmployeeGoals(
                manager.Id,
                employeeId)
            .Select(g =>
                new GoalResponseDto
                {
                    Id = g.Id,

                    EmployeeName =
                        g.Employee!.FirstName +
                        " " +
                        g.Employee.LastName,

                    Title = g.Title,

                    Description = g.Description ?? "",

                    TargetDate = g.TargetDate,

                    Status = g.Status ?? ""
                })
            .ToList();
    }

    public void UpdateGoalStatus(Guid managerUserId,Guid goalId,UpdateGoalStatusDto dto)
    {
        var manager = repository.GetEmployeeByUserId(managerUserId);

        if (manager == null)
        {
            throw new NotFoundException("Manager not found");
        }

        var goal = repository.GetGoal(goalId);

        if (goal == null)
        {
            throw new NotFoundException("Goal not found");
        }

        if (goal.Employee?.ManagerId != manager.Id)
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }

        string[] validStatuses =
        {
            "Assigned",
            "InProgress",
            "Completed",
            "Cancelled"
        };

        if (!validStatuses.Contains(dto.Status))
        {
            throw new BusinessException("Invalid status");
        }

        goal.Status = dto.Status;

        repository.UpdateGoal(goal);

        repository.SaveChanges();
    }


    public List<GoalResponseDto> GetMyGoals(
    Guid employeeUserId)
{
    var employee =
        repository
        .GetEmployeeByUserId(
            employeeUserId);

    if (employee == null)
    {
        throw new NotFoundException(
            "Employee not found");
    }

    return repository
        .GetGoalsByEmployee(
            employee.Id)
        .Select(g =>
            new GoalResponseDto
            {
                Id = g.Id,

                EmployeeName =
                    employee.FirstName +
                    " " +
                    employee.LastName,

                Title = g.Title,

                Description =
                    g.Description ?? "",

                TargetDate =
                    g.TargetDate,

                Status =
                    g.Status ?? ""
            })
        .ToList();
}

public void UpdateMyGoalStatus(
    Guid employeeUserId,
    Guid goalId,
    UpdateGoalStatusDto dto)
{
    var employee =
        repository
        .GetEmployeeByUserId(
            employeeUserId);

    if (employee == null)
    {
        throw new NotFoundException(
            "Employee not found");
    }

    var goal =
        repository.GetEmployeeGoal(
            goalId,
            employee.Id);

    if (goal == null)
    {
        throw new NotFoundException(
            "Goal not found");
    }

    string[] validStatuses =
    {
        "Assigned",
        "InProgress",
        "Completed"
    };

    if (!validStatuses.Contains(
        dto.Status))
    {
        throw new BusinessException(
            "Invalid status");
    }

    goal.Status = dto.Status;

    repository.UpdateGoal(goal);

    repository.SaveChanges();
}
}
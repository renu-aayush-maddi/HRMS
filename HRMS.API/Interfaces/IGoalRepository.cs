using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.Goal;
using HRMS.API.Models.DTOs.Common;

namespace HRMS.API.Interfaces;

public interface IGoalRepository
{
    Employee? GetEmployeeByUserId(Guid userId);

    Employee? GetTeamMember(Guid managerId,Guid employeeId);

    EmployeeGoal? GetGoal(Guid goalId);

    List<EmployeeGoal> GetGoals(Guid managerId,GoalQueryDto query,int skip,int take);

    int GetGoalsCount(Guid managerId,GoalQueryDto query);

    List<EmployeeGoal> GetEmployeeGoals(Guid managerId,Guid employeeId);

    void AddGoal(EmployeeGoal goal);

    void UpdateGoal(EmployeeGoal goal);

    void SaveChanges();

    List<EmployeeGoal> GetGoalsByEmployee(Guid employeeId);

    EmployeeGoal? GetEmployeeGoal(Guid goalId,Guid employeeId);
}
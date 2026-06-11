using HRMS.API.Models.DTOs.Goal;
using HRMS.API.Models.DTOs.Common;
using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IGoalService
{
    void AddGoal(Guid managerUserId,AddGoalDto dto);

    PaginatedResponseDto<GoalResponseDto> GetGoals(Guid managerUserId,GoalQueryDto query);

    List<GoalResponseDto> GetEmployeeGoals(Guid managerUserId,Guid employeeId);

    void UpdateGoalStatus(Guid managerUserId,Guid goalId,UpdateGoalStatusDto dto);
}
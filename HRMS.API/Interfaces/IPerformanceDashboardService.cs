using HRMS.API.Models.DTOs.Dashboard;

namespace HRMS.API.Interfaces;

public interface IPerformanceDashboardService
{
    ManagerDashboardDto GetManagerDashboard(Guid managerUserId,Guid cycleId);
    HrDashboardDto GetHrDashboard(Guid cycleId);
}
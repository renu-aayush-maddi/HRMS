using HRMS.API.Models.DTOs.Dashboard;

namespace HRMS.API.Interfaces;

public interface IHrDashboardRepository
{
    HrDashboardStatsDto GetStats();
    List<DepartmentSummaryDto> GetDepartmentSummary();
    LeaveSummaryDto GetLeaveSummary();
    PayrollSummaryDto GetPayrollSummary();
}
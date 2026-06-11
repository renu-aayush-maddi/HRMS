using HRMS.API.Models.DTOs.Dashboard;

namespace HRMS.API.Interfaces;

public interface IHrDashboardService
{
    HrDashboardStatsDto GetStats();
    List<DepartmentSummaryDto> GetDepartmentSummary();
    LeaveSummaryDto GetLeaveSummary();
    PayrollSummaryDto GetPayrollSummary();
}
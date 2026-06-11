using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Dashboard;


namespace HRMS.API.Services;

public class HrDashboardService
    : IHrDashboardService
{
    private readonly IHrDashboardRepository repository;

    public HrDashboardService(
        IHrDashboardRepository repository)
    {
        this.repository = repository;
    }

    public HrDashboardStatsDto GetStats()
    {
        return repository.GetStats();
    }

    public List<DepartmentSummaryDto> GetDepartmentSummary()
    {
        return repository
            .GetDepartmentSummary();
    }

    public LeaveSummaryDto GetLeaveSummary()
    {
        return repository.GetLeaveSummary();
    }

    public PayrollSummaryDto GetPayrollSummary()
    {
        return repository.GetPayrollSummary();
    }
}
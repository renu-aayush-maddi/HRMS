using HRMS.API.Models.DTOs.EmployeeSalary;

namespace HRMS.API.Interfaces;

public interface IEmployeeSalaryService
{
    Task AssignSalaryAsync(
        AssignEmployeeSalaryDto dto, CancellationToken cancellationToken = default);

    Task<EmployeeSalaryResponseDto>
        GetActiveSalaryAsync(
            Guid employeeId, CancellationToken cancellationToken = default);

    Task<List<EmployeeSalaryResponseDto>>
        GetAllAsync(CancellationToken cancellationToken = default);

    Task<List<SalaryHistoryResponseDto>>
        GetSalaryHistoryAsync(
            Guid employeeId, CancellationToken cancellationToken = default);
}
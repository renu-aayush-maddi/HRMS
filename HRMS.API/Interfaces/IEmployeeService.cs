using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.Employee;
using Microsoft.AspNetCore.Http;

namespace HRMS.API.Interfaces;

public interface IEmployeeService
{
    Task<EmployeeCreatedDto> AddEmployeeAsync(AddEmployeeDto dto, CancellationToken cancellationToken = default);

    Task<EmployeeResponseDto> UpdateEmployeeAsync(Guid employeeId, UpdateEmployeeDto dto, CancellationToken cancellationToken = default);

    Task DeleteEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<EmployeeResponseDto> GetEmployeeByIdAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<PagedResponse<EmployeeResponseDto>> GetEmployeesAsync(EmployeeFilterDto filter, CancellationToken cancellationToken = default);

    Task<EmployeeFullProfileDto> GetEmployeeFullProfileAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<EmployeeFullProfileDto> GetMyProfileAsync(CancellationToken cancellationToken = default);

    Task UpdateEmployeeStatusAsync(Guid employeeId, UpdateEmployeeStatusDto dto, CancellationToken cancellationToken = default);

    Task<List<ManagerLookupDto>> GetManagersAsync(CancellationToken cancellationToken = default);

    Task<byte[]> ExportEmployeesAsync(EmployeeFilterDto filter, CancellationToken cancellationToken = default);

    Task<EmployeeImportResultDto> ImportEmployeesAsync(IFormFile file, CancellationToken cancellationToken = default);
}
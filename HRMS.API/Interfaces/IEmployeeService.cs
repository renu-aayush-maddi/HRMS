using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.Employee;

namespace HRMS.API.Interfaces;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeResponseDto>> GetAllEmployeesAsync(string? search,int page,int pageSize);

    Task<EmployeeResponseDto?> GetEmployeeByIdAsync(Guid id);

    Task<EmployeeCreatedDto> AddEmployeeAsync(AddEmployeeDto dto);

    Task UpdateEmployeeAsync(Guid id, UpdateEmployeeDto dto);

    Task DeleteEmployeeAsync(Guid id);

    Task UpdateEmployeeStatusAsync(Guid employeeId,UpdateEmployeeStatusDto dto);

    Task<EmployeeFullProfileDto?> GetFullProfileAsync(Guid employeeId);
}
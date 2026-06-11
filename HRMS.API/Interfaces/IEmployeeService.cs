using HRMS.API.Models.DTOs.Employee;

namespace HRMS.API.Interfaces;

public interface IEmployeeService
{
    List<EmployeeResponseDto> GetAllEmployees(
        string? search,
        int page,
        int pageSize);

    EmployeeResponseDto? GetEmployeeById(Guid id);

    EmployeeCreatedDto AddEmployee(AddEmployeeDto dto);

    void UpdateEmployee(Guid id, UpdateEmployeeDto dto);

    void DeleteEmployee(Guid id);

    void UpdateEmployeeStatus(Guid employeeId,UpdateEmployeeStatusDto dto);

    EmployeeFullProfileDto? GetFullProfile(Guid employeeId);
}
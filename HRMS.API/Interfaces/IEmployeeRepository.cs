using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IEmployeeRepository
{
    Task<(List<Employee> Employees, int TotalCount)> GetAllEmployeesAsync(string? search,int page,int pageSize);

    Task<Employee?> GetEmployeeByIdAsync(Guid id);

    Task<Employee?> GetEmployeeFullProfileAsync(Guid employeeId);

    Task<bool> EmployeeExistsAsync(string email);

    Task<bool> DepartmentExistsAsync(Guid departmentId);

    Task<Role?> GetRoleByNameAsync(string roleName);

    Task AddEmployeeAsync(Employee employee);

    Task AddUserAsync(User user);

    void UpdateEmployee(Employee employee);

    void SoftDeleteEmployee(Employee employee,Guid deletedBy);

    Task AddAuditLogAsync(AuditLog auditLog);

    Task SaveChangesAsync();
}
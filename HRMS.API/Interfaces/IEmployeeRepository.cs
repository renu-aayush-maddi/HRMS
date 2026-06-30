using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace HRMS.API.Interfaces;

public interface IEmployeeRepository
{
    IQueryable<Employee> GetEmployees();

    Task<Employee?> GetEmployeeByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeFullProfileAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeByCodeAsync(string employeeCode, CancellationToken cancellationToken = default);

    Task<bool> EmployeeExistsAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> DepartmentExistsAsync(Guid departmentId, CancellationToken cancellationToken = default);

    Task<bool> ManagerExistsAsync(Guid managerId, CancellationToken cancellationToken = default);

    Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);

    Task<List<Employee>> GetManagersAsync(CancellationToken cancellationToken = default);

    Task<List<Employee>> GetActiveEmployeesForHierarchyAsync(CancellationToken cancellationToken = default);

    Task<List<Employee>> GetDirectReportsAsync(Guid managerId, CancellationToken cancellationToken = default);

    Task<List<Employee>> SearchActiveEmployeesAsync(string query, CancellationToken cancellationToken = default);

    Task<long> GetNextEmployeeNumberAsync(CancellationToken cancellationToken = default);

    Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken = default);

    Task AddUserAsync(User user, CancellationToken cancellationToken = default);

    void UpdateEmployee(Employee employee);

    void SoftDeleteEmployee(Employee employee, Guid deletedBy);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
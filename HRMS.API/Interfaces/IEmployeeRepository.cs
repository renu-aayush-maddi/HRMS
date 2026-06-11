using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IEmployeeRepository
{
    List<Employee> GetAllEmployees(string? search,int page,int pageSize);

    Employee? GetEmployeeById(Guid id);

    void AddEmployee(Employee employee);

    void UpdateEmployee(Employee employee);

    void DeleteEmployee(Employee employee);

    bool EmployeeExists(string email);

    Role? GetRoleByName(string roleName);

    void AddUser(User user);

    Employee? GetEmployeeFullProfile(Guid employeeId);

    void SaveChanges();
}
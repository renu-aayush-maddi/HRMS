using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext context;

    public EmployeeRepository(AppDbContext context)
    {
        this.context = context;
    }

    public List<Employee> GetAllEmployees(
        string? search,
        int page,
        int pageSize)
    {
        var query = context.Employees
            .Include(e => e.Department)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();

            query = query.Where(e =>
                e.FirstName.ToLower().Contains(search)
                ||
                e.LastName.ToLower().Contains(search)
                ||
                e.Email.ToLower().Contains(search)
            );
        }

        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public Employee? GetEmployeeById(Guid id)
    {
        return context.Employees
            .Include(e => e.Department)
            .FirstOrDefault(e => e.Id == id);
    }

    public void AddEmployee(Employee employee)
    {
        context.Employees.Add(employee);
    }

    public void UpdateEmployee(Employee employee)
    {
        context.Employees.Update(employee);
    }

    public void DeleteEmployee(Employee employee)
    {
        context.Employees.Remove(employee);
    }

    public bool EmployeeExists(string email)
    {
        return context.Employees
            .Any(e => e.Email == email);
    }

    public Role? GetRoleByName(string roleName)
    {
        return context.Roles
            .FirstOrDefault(r => r.Name == roleName);
    }

    public void AddUser(User user)
    {
        context.Users.Add(user);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }

    public Employee? GetEmployeeFullProfile(Guid employeeId)
    {
        return context.Employees
            .Include(e => e.Department)

            .Include(e => e.Manager)

            .Include(e => e.EmployeeEducations)

            .Include(e => e.EmployeeExperiences)

            .Include(e => e.EmployeeEmergencyContacts)

            .Include(e => e.EmployeeAddresses)

            .Include(e => e.EmployeeDocuments)

            .FirstOrDefault(e =>
                e.Id == employeeId);
    }


}
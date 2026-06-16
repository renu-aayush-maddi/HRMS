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

    public async Task<(List<Employee> Employees, int TotalCount)>
        GetAllEmployeesAsync(string? search, int page, int pageSize)
    {
        var query = context.Employees
            .Include(e => e.Department)
            .Where(e => e.IsDeleted == false)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();

            query = query.Where(e =>
                e.FirstName.ToLower().Contains(search)
                ||
                e.LastName.ToLower().Contains(search)
                ||
                e.Email.ToLower().Contains(search));
        }

        var totalCount = await query.CountAsync();

        var employees =
            await query
            .OrderBy(e => e.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (employees, totalCount);
    }

    public async Task<Employee?> GetEmployeeByIdAsync(Guid id)
    {
        return await context.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e =>
            e.Id == id &&
                e.IsDeleted == false);
    }

    public async Task<Employee?> GetEmployeeFullProfileAsync(Guid employeeId)
    {
        return await context.Employees
            .Include(e => e.Department)

            .Include(e => e.Manager)

            .Include(e => e.EmployeeEducations)

            .Include(e => e.EmployeeExperiences)

            .Include(e => e.EmployeeEmergencyContacts)

            .Include(e => e.EmployeeAddresses)

            .Include(e => e.EmployeeDocuments)

            .AsSplitQuery()

            .FirstOrDefaultAsync(e =>
                e.Id == employeeId &&
                e.IsDeleted == false);
    }

    public async Task<bool> EmployeeExistsAsync(string email)
    {
        return await context.Employees
            .AnyAsync(e =>
                e.Email == email &&
                e.IsDeleted == false);
    }

    public async Task<bool> DepartmentExistsAsync(Guid departmentId)
    {
        return await context.Departments.AnyAsync(d =>d.Id == departmentId);
    }

    public async Task<Role?> GetRoleByNameAsync(string roleName)
    {
        return await context.Roles.FirstOrDefaultAsync(r =>r.Name == roleName);
    }

    public async Task AddEmployeeAsync(Employee employee)
    {
        await context.Employees.AddAsync(employee);
    }

    public async Task AddUserAsync(User user)
    {
        await context.Users.AddAsync(user);
    }

    public void UpdateEmployee(Employee employee)
    {
        context.Employees.Update(employee);
    }

    public void SoftDeleteEmployee(Employee employee,Guid deletedBy)
    {
        employee.IsDeleted = true;

        employee.DeletedAt = DateTime.Now;

        employee.DeletedBy = deletedBy;

        context.Employees.Update(employee);
    }

    public async Task AddAuditLogAsync(AuditLog auditLog)
    {
        await context.AuditLogs.AddAsync(auditLog);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }


}

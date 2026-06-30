using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace HRMS.API.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext context;

    public EmployeeRepository(AppDbContext context)
    {
        this.context = context;
    }

    public IQueryable<Employee> GetEmployees()
    {
        return context.Employees
            .AsNoTracking()
            .Include(x => x.Department)
            .Include(x => x.Manager)
            .Where(x => !x.IsDeleted);
    }

    public async Task<Employee?> GetEmployeeByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .Include(x => x.Department)
            .Include(x => x.Manager)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<Employee?> GetEmployeeFullProfileAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .Include(x => x.Department)
            .Include(x => x.Manager)
            .Include(x => x.EmployeeAddresses)
            .Include(x => x.EmployeeEducations)
            .Include(x => x.EmployeeExperiences)
            .Include(x => x.EmployeeEmergencyContacts)
            .Include(x => x.EmployeeDocuments)
            .FirstOrDefaultAsync(x => x.Id == employeeId && !x.IsDeleted, cancellationToken);
    }

    public async Task<Employee?> GetEmployeeByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(x => x.Email == email && !x.IsDeleted, cancellationToken);
    }

    public async Task<Employee?> GetEmployeeByCodeAsync(string employeeCode, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(x => x.EmployeeCode == employeeCode && !x.IsDeleted, cancellationToken);
    }

    public async Task<bool> EmployeeExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .AnyAsync(x => x.Email == email && !x.IsDeleted, cancellationToken);
    }

    public async Task<bool> DepartmentExistsAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        return await context.Departments.AnyAsync(x => x.Id == departmentId, cancellationToken);
    }

    public async Task<bool> ManagerExistsAsync(Guid managerId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .AnyAsync(x => x.Id == managerId && !x.IsDeleted && x.EmploymentStatus == "Active", cancellationToken);
    }

    public async Task<Role?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await context.Roles.FirstOrDefaultAsync(x => x.Name == roleName, cancellationToken);
    }

    public async Task<List<Employee>> GetManagersAsync(
        CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .Include(x => x.User)
            .ThenInclude(x => x.Roles)
            .Where(x =>
                !x.IsDeleted &&
                x.EmploymentStatus == "Active" &&
                x.User != null &&
                x.User.Roles.Any(r => r.Name == "Manager"))
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Employee>> GetActiveEmployeesForHierarchyAsync(CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Where(e => !e.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Employee>> GetDirectReportsAsync(Guid managerId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Where(e => !e.IsDeleted && e.EmploymentStatus == "Active" && e.ManagerId == managerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Employee>> SearchActiveEmployeesAsync(string query, CancellationToken cancellationToken = default)
    {
        var cleanQuery = query.Trim().ToLower();
        return await context.Employees
            .AsNoTracking()
            .Include(e => e.Department)
            .Where(e => !e.IsDeleted && e.EmploymentStatus == "Active")
            .Where(e => e.FirstName.ToLower().Contains(cleanQuery)
                     || e.LastName.ToLower().Contains(cleanQuery)
                     || e.EmployeeCode.ToLower().Contains(cleanQuery)
                     || e.Designation.ToLower().Contains(cleanQuery)
                     || (e.Department != null && e.Department.Name.ToLower().Contains(cleanQuery)))
            .ToListAsync(cancellationToken);
    }

    public async Task<long> GetNextEmployeeNumberAsync(CancellationToken cancellationToken = default)
    {
        var employeeCodes = await context.Employees
            .Where(x => !x.IsDeleted && !string.IsNullOrWhiteSpace(x.EmployeeCode))
            .Select(x => x.EmployeeCode)
            .ToListAsync(cancellationToken);

        if (!employeeCodes.Any())
        {
            return 1001;
        }

        var maxNumber = employeeCodes
            .Select(code =>
            {
                var numeric = new string(code.Where(char.IsDigit).ToArray());
                return long.TryParse(numeric, out var result) ? result : 1000;
            })
            .Max();

        return maxNumber + 1;
    }

    public async Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken = default)
    {
        await context.Employees.AddAsync(employee, cancellationToken);
    }

    public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
    }

    public void UpdateEmployee(Employee employee)
    {
        context.Employees.Update(employee);
    }

    public void SoftDeleteEmployee(Employee employee, Guid deletedBy)
    {
        employee.IsDeleted = true;
        employee.DeletedAt = DateTime.UtcNow;
        employee.DeletedBy = deletedBy;
        context.Employees.Update(employee);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
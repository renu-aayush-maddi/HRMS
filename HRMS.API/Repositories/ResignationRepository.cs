using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class EmployeeResignationRepository : IEmployeeResignationRepository
{
    private readonly AppDbContext context;

    public EmployeeResignationRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<EmployeeResignation?> GetByIdAsync(Guid resignationId, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeResignations
            .FirstOrDefaultAsync(x => x.Id == resignationId, cancellationToken);
    }

    public async Task<EmployeeResignation?> GetByIdWithEmployeeAsync(Guid resignationId, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeResignations
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x => x.Id == resignationId, cancellationToken);
    }

    public async Task<EmployeeResignation?> GetActiveResignationAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeResignations
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && (x.Status == "Pending" || x.Status == "Approved"), cancellationToken);
    }

    public async Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(x => x.Id == employeeId, cancellationToken);
    }

    public IQueryable<EmployeeResignation> GetQueryable()
    {
        return context.EmployeeResignations
            .Include(x => x.Employee)
            .AsQueryable();
    }

    public async Task AddAsync(EmployeeResignation resignation, CancellationToken cancellationToken = default)
    {
        await context.EmployeeResignations.AddAsync(resignation, cancellationToken);
    }

    public Task UpdateAsync(EmployeeResignation resignation, CancellationToken cancellationToken = default)
    {
        context.EmployeeResignations.Update(resignation);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateEmployeeAsync(Employee employee,CancellationToken cancellationToken = default)
    {
        context.Employees.Update(employee);

        return Task.CompletedTask;
    }
}
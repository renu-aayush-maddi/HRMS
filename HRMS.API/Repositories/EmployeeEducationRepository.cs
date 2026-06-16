using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class EmployeeEducationRepository : IEmployeeEducationRepository
{
    private readonly AppDbContext context;

    public EmployeeEducationRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted, cancellationToken);
    }

    public async Task<EmployeeEducation?> GetEducationAsync(Guid educationId, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeEducations
            .FirstOrDefaultAsync(x => x.Id == educationId, cancellationToken);
    }

    public IQueryable<EmployeeEducation> GetEducations()
    {
        return context.EmployeeEducations.AsNoTracking();
    }

    public async Task<bool> EducationExistsAsync(Guid employeeId, string degree, string institutionName, int graduationYear, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeEducations
            .AnyAsync(x => x.EmployeeId == employeeId && 
                           x.Degree == degree && 
                           x.InstitutionName == institutionName && 
                           x.GraduationYear == graduationYear, cancellationToken);
    }

    public async Task<bool> EducationExistsAsync(Guid employeeId, Guid educationId, string degree, string institutionName, int graduationYear, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeEducations
            .AnyAsync(x => x.EmployeeId == employeeId && 
                           x.Id != educationId && 
                           x.Degree == degree && 
                           x.InstitutionName == institutionName && 
                           x.GraduationYear == graduationYear, cancellationToken);
    }

    public async Task AddEducationAsync(EmployeeEducation education, CancellationToken cancellationToken = default)
    {
        await context.EmployeeEducations.AddAsync(education, cancellationToken);
    }

    public void UpdateEducation(EmployeeEducation education)
    {
        context.EmployeeEducations.Update(education);
    }

    public void DeleteEducation(EmployeeEducation education)
    {
        context.EmployeeEducations.Remove(education);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
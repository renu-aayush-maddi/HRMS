using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class EmployeeExperienceRepository : IEmployeeExperienceRepository
{
    private readonly AppDbContext context;

    public EmployeeExperienceRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(x => x.Id == employeeId && !x.IsDeleted, cancellationToken);
    }

    public async Task<EmployeeExperience?> GetExperienceAsync(Guid experienceId, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeExperiences
            .FirstOrDefaultAsync(x => x.Id == experienceId, cancellationToken);
    }

    public IQueryable<EmployeeExperience> GetExperiences()
    {
        return context.EmployeeExperiences.AsNoTracking();
    }

    public async Task<bool> ExperienceExistsAsync(Guid employeeId, string companyName, string designation, DateOnly? startDate, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeExperiences
            .AnyAsync(x => x.EmployeeId == employeeId && 
                           x.CompanyName == companyName && 
                           x.Designation == designation && 
                           x.StartDate == startDate, cancellationToken);
    }

    public async Task<bool> ExperienceExistsAsync(Guid employeeId, Guid experienceId, string companyName, string designation, DateOnly? startDate, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeExperiences
            .AnyAsync(x => x.EmployeeId == employeeId && 
                           x.Id != experienceId && 
                           x.CompanyName == companyName && 
                           x.Designation == designation && 
                           x.StartDate == startDate, cancellationToken);
    }

    public async Task AddExperienceAsync(EmployeeExperience experience, CancellationToken cancellationToken = default)
    {
        await context.EmployeeExperiences.AddAsync(experience, cancellationToken);
    }

    public void UpdateExperience(EmployeeExperience experience)
    {
        context.EmployeeExperiences.Update(experience);
    }

    public void DeleteExperience(EmployeeExperience experience)
    {
        context.EmployeeExperiences.Remove(experience);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
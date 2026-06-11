using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;

namespace HRMS.API.Repositories;

public class EmployeeExperienceRepository
    : IEmployeeExperienceRepository
{
    private readonly AppDbContext context;

    public EmployeeExperienceRepository(
        AppDbContext context)
    {
        this.context = context;
    }

    public Employee? GetEmployee(Guid employeeId)
    {
        return context.Employees
            .FirstOrDefault(e =>
                e.Id == employeeId);
    }

    public EmployeeExperience? GetExperience(Guid id)
    {
        return context.EmployeeExperiences
            .FirstOrDefault(x =>
                x.Id == id);
    }

    public List<EmployeeExperience>
        GetEmployeeExperiences(Guid employeeId)
    {
        return context.EmployeeExperiences
            .Where(x =>
                x.EmployeeId == employeeId)
            .ToList();
    }

    public void AddExperience(
        EmployeeExperience experience)
    {
        context.EmployeeExperiences
            .Add(experience);
    }

    public void UpdateExperience(
        EmployeeExperience experience)
    {
        context.EmployeeExperiences
            .Update(experience);
    }

    public void DeleteExperience(
        EmployeeExperience experience)
    {
        context.EmployeeExperiences
            .Remove(experience);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}
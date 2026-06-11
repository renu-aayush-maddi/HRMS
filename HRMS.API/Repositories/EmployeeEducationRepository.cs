using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;

namespace HRMS.API.Repositories;

public class EmployeeEducationRepository
    : IEmployeeEducationRepository
{
    private readonly AppDbContext context;

    public EmployeeEducationRepository(
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

    public EmployeeEducation? GetEducation(Guid id)
    {
        return context.EmployeeEducations
            .FirstOrDefault(x =>
                x.Id == id);
    }

    public List<EmployeeEducation>
        GetEmployeeEducations(Guid employeeId)
    {
        return context.EmployeeEducations
            .Where(x =>
                x.EmployeeId == employeeId)
            .ToList();
    }

    public void AddEducation(
        EmployeeEducation education)
    {
        context.EmployeeEducations
            .Add(education);
    }

    public void UpdateEducation(
        EmployeeEducation education)
    {
        context.EmployeeEducations
            .Update(education);
    }

    public void DeleteEducation(
        EmployeeEducation education)
    {
        context.EmployeeEducations
            .Remove(education);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}
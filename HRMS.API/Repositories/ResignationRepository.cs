using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class ResignationRepository
    : IResignationRepository
{
    private readonly AppDbContext context;

    public ResignationRepository(
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

    public EmployeeResignation? GetResignation(Guid id)
    {
        return context.EmployeeResignations
            .Include(r => r.Employee)
            .FirstOrDefault(r =>
                r.Id == id);
    }

    public List<EmployeeResignation> GetAll()
    {
        return context.EmployeeResignations
            .Include(r => r.Employee)
            .ToList();
    }

    public List<EmployeeResignation>
        GetEmployeeResignations(
            Guid employeeId)
    {
        return context.EmployeeResignations
            .Include(r => r.Employee)
            .Where(r =>
                r.EmployeeId == employeeId)
            .ToList();
    }

    public void Add(
        EmployeeResignation resignation)
    {
        context.EmployeeResignations
            .Add(resignation);
    }

    public void Update(
        EmployeeResignation resignation)
    {
        context.EmployeeResignations
            .Update(resignation);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}
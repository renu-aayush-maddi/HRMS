using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;

namespace HRMS.API.Repositories;

public class DepartmentRepository: IDepartmentRepository
{
    private readonly AppDbContext context;

    public DepartmentRepository(AppDbContext context)
    {
        this.context = context;
    }

    public List<Department> GetAllDepartments()
    {
        return context.Departments.ToList();
    }

    public Department? GetDepartmentById(Guid id)
    {
        return context.Departments.FirstOrDefault(d => d.Id == id);
    }

    public bool DepartmentExists(string name)
    {
        return context.Departments.Any(d =>d.Name.ToLower() == name.ToLower());
    }

    public void AddDepartment(Department department)
    {
        context.Departments.Add(department);
    }

    public void UpdateDepartment(Department department)
    {
        context.Departments.Update(department);
    }

    public void DeleteDepartment(Department department)
    {
        context.Departments.Remove(department);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }

    public bool HasEmployees(Guid departmentId)
    {
        return context.Employees
            .Any(e => e.DepartmentId == departmentId);
    }
}
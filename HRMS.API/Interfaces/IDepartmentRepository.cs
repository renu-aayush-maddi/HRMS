using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IDepartmentRepository
{
    List<Department> GetAllDepartments();

    Department? GetDepartmentById(Guid id);

    bool DepartmentExists(string name);

    void AddDepartment(Department department);

    void UpdateDepartment(Department department);

    void DeleteDepartment(Department department);

    bool HasEmployees(Guid departmentId);

    void SaveChanges();
}
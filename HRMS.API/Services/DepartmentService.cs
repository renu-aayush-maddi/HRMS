using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Department;
using HRMS.API.Models.Entities;
using HRMS.API.Exceptions;

namespace HRMS.API.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository departmentRepository;

    public DepartmentService(IDepartmentRepository departmentRepository)
    {
        this.departmentRepository = departmentRepository;
    }

    public List<DepartmentResponseDto> GetAllDepartments()
    {
        return departmentRepository
            .GetAllDepartments()
            .Select(d => new DepartmentResponseDto
            {
                Id = d.Id,
                Name = d.Name
            })
            .ToList();
    }

    public void AddDepartment(AddDepartmentDto dto)
    {
        bool exists = departmentRepository.DepartmentExists(dto.Name);

        if (exists)
        {
            throw new BusinessException("Department already exists");
        }

        Department department = new Department
            {
                Id = Guid.NewGuid(),
                Name = dto.Name
            };

        departmentRepository.AddDepartment(department);
        departmentRepository.SaveChanges();
    }

    public void UpdateDepartment(Guid id,UpdateDepartmentDto dto)
    {
        var department =departmentRepository.GetDepartmentById(id);

        if (department == null)
        {
            throw new NotFoundException("Department not found");
        }
        if (department.Name.ToLower()!= dto.Name.ToLower() && departmentRepository.DepartmentExists(dto.Name))
        {
            throw new BusinessException("Department already exists");
        }

        department.Name = dto.Name;

        departmentRepository.UpdateDepartment(department);

        departmentRepository.SaveChanges();
    }

    public void DeleteDepartment(Guid id)
    {
        var department = departmentRepository.GetDepartmentById(id);

        if (department == null)
        {
            throw new NotFoundException("Department not found");
        }


        var hasEmployees = departmentRepository.HasEmployees(id);

        if (hasEmployees)
        {
            throw new BusinessException("Cannot delete department with employees");
        }

        departmentRepository.DeleteDepartment(department);

        departmentRepository.SaveChanges();
    }
}
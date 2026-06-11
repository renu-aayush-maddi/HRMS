using HRMS.API.Models.DTOs.Department;

namespace HRMS.API.Interfaces;

public interface IDepartmentService
{
    List<DepartmentResponseDto> GetAllDepartments();

    void AddDepartment(AddDepartmentDto dto);

    void UpdateDepartment(Guid id,UpdateDepartmentDto dto);

    void DeleteDepartment(Guid id);

    
}
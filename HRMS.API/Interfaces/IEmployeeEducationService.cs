using HRMS.API.Models.DTOs.EmployeeEducation;

namespace HRMS.API.Interfaces;

public interface IEmployeeEducationService
{
    void AddEducation(
        AddEmployeeEducationDto dto);

    List<EmployeeEducationResponseDto>
        GetEmployeeEducations(
            Guid employeeId);

    void UpdateEducation(
        Guid id,
        UpdateEmployeeEducationDto dto);

    void DeleteEducation(Guid id);
}
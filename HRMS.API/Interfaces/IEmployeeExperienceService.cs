using HRMS.API.Models.DTOs.EmployeeExperience;

namespace HRMS.API.Interfaces;

public interface IEmployeeExperienceService
{
    void AddExperience(
        AddEmployeeExperienceDto dto);

    List<EmployeeExperienceResponseDto>
        GetEmployeeExperiences(
            Guid employeeId);

    void UpdateExperience(
        Guid id,
        UpdateEmployeeExperienceDto dto);

    void DeleteExperience(Guid id);
}
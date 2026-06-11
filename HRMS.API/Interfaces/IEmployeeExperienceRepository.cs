using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IEmployeeExperienceRepository
{
    Employee? GetEmployee(Guid employeeId);

    EmployeeExperience? GetExperience(Guid id);

    List<EmployeeExperience>
        GetEmployeeExperiences(Guid employeeId);

    void AddExperience(
        EmployeeExperience experience);

    void UpdateExperience(
        EmployeeExperience experience);

    void DeleteExperience(
        EmployeeExperience experience);

    void SaveChanges();
}
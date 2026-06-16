using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IEmployeeExperienceRepository
{
    Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<EmployeeExperience?> GetExperienceAsync(Guid experienceId, CancellationToken cancellationToken = default);

    IQueryable<EmployeeExperience> GetExperiences();

    Task<bool> ExperienceExistsAsync(Guid employeeId, string companyName, string designation, DateOnly? startDate, CancellationToken cancellationToken = default);

    Task<bool> ExperienceExistsAsync(Guid employeeId, Guid experienceId, string companyName, string designation, DateOnly? startDate, CancellationToken cancellationToken = default);

    Task AddExperienceAsync(EmployeeExperience experience, CancellationToken cancellationToken = default);

    void UpdateExperience(EmployeeExperience experience);

    void DeleteExperience(EmployeeExperience experience);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
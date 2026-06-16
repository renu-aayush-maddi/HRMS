using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IEmployeeEducationRepository
{
    Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<EmployeeEducation?> GetEducationAsync(Guid educationId, CancellationToken cancellationToken = default);

    IQueryable<EmployeeEducation> GetEducations();

    Task<bool> EducationExistsAsync(Guid employeeId, string degree, string institutionName, int graduationYear, CancellationToken cancellationToken = default);

    Task<bool> EducationExistsAsync(Guid employeeId, Guid educationId, string degree, string institutionName, int graduationYear, CancellationToken cancellationToken = default);

    Task AddEducationAsync(EmployeeEducation education, CancellationToken cancellationToken = default);

    void UpdateEducation(EmployeeEducation education);

    void DeleteEducation(EmployeeEducation education);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
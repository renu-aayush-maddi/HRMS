using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IEmployeeResignationRepository
{
    Task<EmployeeResignation?> GetByIdAsync(Guid resignationId, CancellationToken cancellationToken = default);

    Task<EmployeeResignation?> GetByIdWithEmployeeAsync(Guid resignationId, CancellationToken cancellationToken = default);

    Task<EmployeeResignation?> GetActiveResignationAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);

    IQueryable<EmployeeResignation> GetQueryable();

    Task AddAsync(EmployeeResignation resignation, CancellationToken cancellationToken = default);

    Task UpdateAsync(EmployeeResignation resignation, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task UpdateEmployeeAsync(Employee employee,CancellationToken cancellationToken = default);
}
using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IEmployeeEmergencyContactRepository
{
    Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<EmployeeEmergencyContact?> GetContactAsync(Guid contactId, CancellationToken cancellationToken = default);

    IQueryable<EmployeeEmergencyContact> GetContacts();

    Task<int> GetContactCountAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<bool> PhoneExistsAsync(Guid employeeId, string phone, CancellationToken cancellationToken = default);

    Task<bool> PhoneExistsAsync(Guid employeeId, Guid contactId, string phone, CancellationToken cancellationToken = default);

    Task AddContactAsync(EmployeeEmergencyContact contact, CancellationToken cancellationToken = default);

    void UpdateContact(EmployeeEmergencyContact contact);

    void DeleteContact(EmployeeEmergencyContact contact);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

}
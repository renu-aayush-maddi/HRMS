using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IEmployeeAddressRepository
{
    Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<EmployeeAddress?> GetAddressAsync(Guid addressId, CancellationToken cancellationToken = default);

    IQueryable<EmployeeAddress> GetAddresses();

    Task AddAddressAsync(EmployeeAddress address, CancellationToken cancellationToken = default);

    void UpdateAddress(EmployeeAddress address);

    void DeleteAddress(EmployeeAddress address);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<bool> AddressTypeExistsAsync(Guid employeeId, string addressType, CancellationToken cancellationToken = default);

    Task<bool> AddressTypeExistsAsync(Guid employeeId, Guid addressId, string addressType, CancellationToken cancellationToken = default);
}
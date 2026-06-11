using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IEmployeeAddressRepository
{
    Employee? GetEmployee(Guid employeeId);

    EmployeeAddress? GetAddress(Guid id);

    List<EmployeeAddress>
        GetEmployeeAddresses(Guid employeeId);

    void AddAddress(
        EmployeeAddress address);

    void UpdateAddress(
        EmployeeAddress address);

    void DeleteAddress(
        EmployeeAddress address);

    void SaveChanges();
}
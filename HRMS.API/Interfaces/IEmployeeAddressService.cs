using HRMS.API.Models.DTOs.EmployeeAddress;

namespace HRMS.API.Interfaces;

public interface IEmployeeAddressService
{
    void AddAddress(
        AddEmployeeAddressDto dto);

    List<EmployeeAddressResponseDto>
        GetEmployeeAddresses(
            Guid employeeId);

    void UpdateAddress(
        Guid id,
        UpdateEmployeeAddressDto dto);

    void DeleteAddress(Guid id);
}
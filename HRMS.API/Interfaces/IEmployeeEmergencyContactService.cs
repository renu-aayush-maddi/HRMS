using HRMS.API.Models.DTOs.EmployeeEmergencyContact;

namespace HRMS.API.Interfaces;

public interface IEmployeeEmergencyContactService
{
    void AddContact(
        AddEmployeeEmergencyContactDto dto);

    List<EmployeeEmergencyContactResponseDto>
        GetEmployeeContacts(
            Guid employeeId);

    void UpdateContact(
        Guid id,
        UpdateEmployeeEmergencyContactDto dto);

    void DeleteContact(Guid id);
}
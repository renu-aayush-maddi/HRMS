using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IEmployeeEmergencyContactRepository
{
    Employee? GetEmployee(Guid employeeId);

    EmployeeEmergencyContact?
        GetContact(Guid id);

    List<EmployeeEmergencyContact>
        GetEmployeeContacts(
            Guid employeeId);

    void AddContact(
        EmployeeEmergencyContact contact);

    void UpdateContact(
        EmployeeEmergencyContact contact);

    void DeleteContact(
        EmployeeEmergencyContact contact);

    void SaveChanges();
}
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.EmployeeEmergencyContact;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

public class EmployeeEmergencyContactService
    : IEmployeeEmergencyContactService
{
    private readonly
        IEmployeeEmergencyContactRepository repository;

    public EmployeeEmergencyContactService(
        IEmployeeEmergencyContactRepository repository)
    {
        this.repository = repository;
    }

    public void AddContact(
        AddEmployeeEmergencyContactDto dto)
    {
        var employee =
            repository.GetEmployee(
                dto.EmployeeId);

        if(employee == null)
        {
            throw new Exception(
                "Employee not found");
        }

        EmployeeEmergencyContact contact =
            new()
            {
                Id = Guid.NewGuid(),

                EmployeeId =
                    dto.EmployeeId,

                ContactName =
                    dto.ContactName,

                Relationship =
                    dto.Relationship,

                Phone =
                    dto.Phone,

                Email =
                    dto.Email
            };

        repository.AddContact(contact);

        repository.SaveChanges();
    }

    public List<EmployeeEmergencyContactResponseDto>
        GetEmployeeContacts(
            Guid employeeId)
    {
        return repository
            .GetEmployeeContacts(employeeId)
            .Select(c =>
                new EmployeeEmergencyContactResponseDto
                {
                    Id = c.Id,

                    ContactName =
                        c.ContactName ?? "",

                    Relationship =
                        c.Relationship ?? "",

                    Phone =
                        c.Phone ?? "",

                    Email =
                        c.Email ?? ""
                })
            .ToList();
    }

    public void UpdateContact(
        Guid id,
        UpdateEmployeeEmergencyContactDto dto)
    {
        var contact =
            repository.GetContact(id);

        if(contact == null)
        {
            throw new Exception(
                "Contact not found");
        }

        contact.ContactName =
            dto.ContactName;

        contact.Relationship =
            dto.Relationship;

        contact.Phone =
            dto.Phone;

        contact.Email =
            dto.Email;

        repository.UpdateContact(contact);

        repository.SaveChanges();
    }

    public void DeleteContact(Guid id)
    {
        var contact =
            repository.GetContact(id);

        if(contact == null)
        {
            throw new Exception(
                "Contact not found");
        }

        repository.DeleteContact(contact);

        repository.SaveChanges();
    }
}
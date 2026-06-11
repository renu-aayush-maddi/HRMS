using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;

namespace HRMS.API.Repositories;

public class EmployeeEmergencyContactRepository
    : IEmployeeEmergencyContactRepository
{
    private readonly AppDbContext context;

    public EmployeeEmergencyContactRepository(
        AppDbContext context)
    {
        this.context = context;
    }

    public Employee? GetEmployee(
        Guid employeeId)
    {
        return context.Employees
            .FirstOrDefault(e =>
                e.Id == employeeId);
    }

    public EmployeeEmergencyContact?
        GetContact(Guid id)
    {
        return context.EmployeeEmergencyContacts
            .FirstOrDefault(x =>
                x.Id == id);
    }

    public List<EmployeeEmergencyContact>
        GetEmployeeContacts(
            Guid employeeId)
    {
        return context.EmployeeEmergencyContacts
            .Where(x =>
                x.EmployeeId == employeeId)
            .ToList();
    }

    public void AddContact(
        EmployeeEmergencyContact contact)
    {
        context.EmployeeEmergencyContacts
            .Add(contact);
    }

    public void UpdateContact(
        EmployeeEmergencyContact contact)
    {
        context.EmployeeEmergencyContacts
            .Update(contact);
    }

    public void DeleteContact(
        EmployeeEmergencyContact contact)
    {
        context.EmployeeEmergencyContacts
            .Remove(contact);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}
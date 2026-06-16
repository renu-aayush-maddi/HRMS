using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class EmployeeEmergencyContactRepository : IEmployeeEmergencyContactRepository
{
    private readonly AppDbContext context;

    public EmployeeEmergencyContactRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(x => x.Id == employeeId && !x.IsDeleted, cancellationToken);
    }

    public async Task<EmployeeEmergencyContact?> GetContactAsync(Guid contactId, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeEmergencyContacts
            .FirstOrDefaultAsync(x => x.Id == contactId, cancellationToken);
    }

    public IQueryable<EmployeeEmergencyContact> GetContacts()
    {
        return context.EmployeeEmergencyContacts.AsNoTracking();
    }

    public async Task<int> GetContactCountAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeEmergencyContacts
            .CountAsync(x => x.EmployeeId == employeeId, cancellationToken);
    }

    public async Task<bool> PhoneExistsAsync(Guid employeeId, string phone, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeEmergencyContacts
            .AnyAsync(x => x.EmployeeId == employeeId && x.Phone == phone, cancellationToken);
    }

    public async Task<bool> PhoneExistsAsync(Guid employeeId, Guid contactId, string phone, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeEmergencyContacts
            .AnyAsync(x => x.EmployeeId == employeeId && x.Id != contactId && x.Phone == phone, cancellationToken);
    }

    public async Task AddContactAsync(EmployeeEmergencyContact contact, CancellationToken cancellationToken = default)
    {
        await context.EmployeeEmergencyContacts.AddAsync(contact, cancellationToken);
    }

    public void UpdateContact(EmployeeEmergencyContact contact)
    {
        context.EmployeeEmergencyContacts.Update(contact);
    }

    public void DeleteContact(EmployeeEmergencyContact contact)
    {
        context.EmployeeEmergencyContacts.Remove(contact);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
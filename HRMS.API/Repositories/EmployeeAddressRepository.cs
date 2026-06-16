using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class EmployeeAddressRepository : IEmployeeAddressRepository
{
    private readonly AppDbContext context;

    public EmployeeAddressRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted, cancellationToken);
    }

    public async Task<EmployeeAddress?> GetAddressAsync(Guid addressId, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeAddresses
            .FirstOrDefaultAsync(a => a.Id == addressId, cancellationToken);
    }

    public IQueryable<EmployeeAddress> GetAddresses()
    {
        return context.EmployeeAddresses.AsNoTracking();
    }

    public async Task AddAddressAsync(EmployeeAddress address, CancellationToken cancellationToken = default)
    {
        await context.EmployeeAddresses.AddAsync(address, cancellationToken);
    }

    public void UpdateAddress(EmployeeAddress address)
    {
        context.EmployeeAddresses.Update(address);
    }

    public void DeleteAddress(EmployeeAddress address)
    {
        context.EmployeeAddresses.Remove(address);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> AddressTypeExistsAsync(Guid employeeId,string addressType,CancellationToken cancellationToken = default)
    {
        return await context.EmployeeAddresses
            .AnyAsync(
                x =>
                    x.EmployeeId == employeeId &&
                    x.AddressType == addressType,
                    cancellationToken);
    }

    public async Task<bool> AddressTypeExistsAsync(Guid employeeId,Guid addressId,string addressType,CancellationToken cancellationToken = default)
    {
        return await context.EmployeeAddresses
            .AnyAsync(
                x =>
                    x.EmployeeId == employeeId &&
                    x.Id != addressId &&
                    x.AddressType == addressType,
                cancellationToken);
    }
}
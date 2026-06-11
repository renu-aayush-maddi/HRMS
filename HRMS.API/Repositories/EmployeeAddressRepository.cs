using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;

namespace HRMS.API.Repositories;

public class EmployeeAddressRepository
    : IEmployeeAddressRepository
{
    private readonly AppDbContext context;

    public EmployeeAddressRepository(
        AppDbContext context)
    {
        this.context = context;
    }

    public Employee? GetEmployee(Guid employeeId)
    {
        return context.Employees
            .FirstOrDefault(e =>
                e.Id == employeeId);
    }

    public EmployeeAddress? GetAddress(Guid id)
    {
        return context.EmployeeAddresses
            .FirstOrDefault(a =>
                a.Id == id);
    }

    public List<EmployeeAddress>
        GetEmployeeAddresses(Guid employeeId)
    {
        return context.EmployeeAddresses
            .Where(a =>
                a.EmployeeId == employeeId)
            .ToList();
    }

    public void AddAddress(
        EmployeeAddress address)
    {
        context.EmployeeAddresses
            .Add(address);
    }

    public void UpdateAddress(
        EmployeeAddress address)
    {
        context.EmployeeAddresses
            .Update(address);
    }

    public void DeleteAddress(
        EmployeeAddress address)
    {
        context.EmployeeAddresses
            .Remove(address);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}
using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class EmployeeSalaryRepository
    : IEmployeeSalaryRepository
{
    private readonly AppDbContext context;

    public EmployeeSalaryRepository(
        AppDbContext context)
    {
        this.context = context;
    }

    public Employee? GetEmployee(
        Guid employeeId)
    {
        return context.Employees
            .FirstOrDefault(x =>
                x.Id == employeeId);
    }

    public SalaryStructure? GetSalaryStructure(
        Guid salaryStructureId)
    {
        return context.SalaryStructures
            .FirstOrDefault(x =>
                x.Id == salaryStructureId);
    }

    public EmployeeSalary? GetActiveSalary(
        Guid employeeId)
    {
        return context.EmployeeSalaries
            .Include(x => x.Employee)
            .Include(x => x.SalaryStructure)
            .FirstOrDefault(x =>
                x.EmployeeId == employeeId
                &&
                x.IsActive == true);
    }

    public List<EmployeeSalary> GetAll()
    {
        return context.EmployeeSalaries
            .Include(x => x.Employee)
            .Include(x => x.SalaryStructure)
            .ToList();
    }

    public void Add(
        EmployeeSalary employeeSalary)
    {
        context.EmployeeSalaries
            .Add(employeeSalary);
    }

    public void Update(
        EmployeeSalary employeeSalary)
    {
        context.EmployeeSalaries
            .Update(employeeSalary);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }

        public List<EmployeeSalary> GetSalaryHistory(
    Guid employeeId)
    {
        return context.EmployeeSalaries
            .Include(x => x.Employee)
            .Include(x => x.SalaryStructure)
            .Where(x =>
                x.EmployeeId == employeeId)
            .OrderByDescending(x =>
                x.EffectiveFrom)
            .ToList();
    }
}
using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class DeductionRepository
    : IDeductionRepository
{
    private readonly AppDbContext context;

    public DeductionRepository(
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

    public Employee? GetEmployeeByUserId(
        Guid userId)
    {
        return context.Employees
            .FirstOrDefault(x =>
                x.UserId == userId);
    }

    public Deduction? GetDeduction(
        Guid deductionId)
    {
        return context.Deductions
            .Include(x => x.Employee)
            .FirstOrDefault(x =>
                x.Id == deductionId);
    }

    public List<Deduction> GetAllDeductions()
    {
        return context.Deductions
            .Include(x => x.Employee)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }

    public List<Deduction> GetEmployeeDeductions(
        Guid employeeId)
    {
        return context.Deductions
            .Include(x => x.Employee)
            .Where(x =>
                x.EmployeeId == employeeId)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }

    public void AddDeduction(
        Deduction deduction)
    {
        context.Deductions.Add(
            deduction);
    }

    public void UpdateDeduction(
        Deduction deduction)
    {
        context.Deductions.Update(
            deduction);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}
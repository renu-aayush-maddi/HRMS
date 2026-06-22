using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class DeductionRepository : IDeductionRepository
{
    private readonly AppDbContext context;

    public DeductionRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(x => x.Id == employeeId && !x.IsDeleted, cancellationToken);
    }

    public async Task<Employee?> GetEmployeeByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted, cancellationToken);
    }

    public async Task<Deduction?> GetDeductionAsync(Guid deductionId, CancellationToken cancellationToken = default)
    {
        return await context.Deductions
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x => x.Id == deductionId, cancellationToken);
    }

    public IQueryable<Deduction> GetDeductions()
    {
        return context.Deductions
            .AsNoTracking()
            .Include(x => x.Employee);
    }

    public async Task<bool> DeductionExistsAsync(Guid employeeId, string reason, int deductionMonth, int deductionYear, CancellationToken cancellationToken = default)
    {
        return await context.Deductions
            .AnyAsync(x => x.EmployeeId == employeeId &&
                           x.DeductionMonth == deductionMonth &&
                           x.DeductionYear == deductionYear &&
                           x.Reason.ToLower() == reason.ToLower(), cancellationToken);
    }

    public async Task AddDeductionAsync(Deduction deduction, CancellationToken cancellationToken = default)
    {
        await context.Deductions.AddAsync(deduction, cancellationToken);
    }

    public void UpdateDeduction(Deduction deduction)
    {
        context.Deductions.Update(deduction);
    }

    public void DeleteDeduction(Deduction deduction)
    {
        context.Deductions.Remove(deduction);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
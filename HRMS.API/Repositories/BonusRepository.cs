using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class BonusRepository : IBonusRepository
{
    private readonly AppDbContext context;

    public BonusRepository(AppDbContext context)
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

    public async Task<Bonuse?> GetBonusAsync(Guid bonusId, CancellationToken cancellationToken = default)
    {
        return await context.Bonuses
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x => x.Id == bonusId, cancellationToken);
    }

    public IQueryable<Bonuse> GetBonuses()
    {
        return context.Bonuses
            .AsNoTracking()
            .Include(x => x.Employee);
    }

    public async Task<bool> BonusExistsAsync(Guid employeeId, string reason, int bonusMonth, int bonusYear, CancellationToken cancellationToken = default)
    {
        return await context.Bonuses
            .AnyAsync(x => x.EmployeeId == employeeId &&
                           x.BonusMonth == bonusMonth &&
                           x.BonusYear == bonusYear &&
                           x.Reason.ToLower() == reason.ToLower(), cancellationToken);
    }

    public async Task AddBonusAsync(Bonuse bonus, CancellationToken cancellationToken = default)
    {
        await context.Bonuses.AddAsync(bonus, cancellationToken);
    }

    public void UpdateBonus(Bonuse bonus)
    {
        context.Bonuses.Update(bonus);
    }

    public void DeleteBonus(Bonuse bonus)
    {
        context.Bonuses.Remove(bonus);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
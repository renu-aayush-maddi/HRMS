using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IBonusRepository
{
    Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Bonuse?> GetBonusAsync(Guid bonusId, CancellationToken cancellationToken = default);

    IQueryable<Bonuse> GetBonuses();

    Task<bool> BonusExistsAsync(Guid employeeId, string reason, int bonusMonth, int bonusYear, CancellationToken cancellationToken = default);

    Task AddBonusAsync(Bonuse bonus, CancellationToken cancellationToken = default);

    void UpdateBonus(Bonuse bonus);

    void DeleteBonus(Bonuse bonus);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
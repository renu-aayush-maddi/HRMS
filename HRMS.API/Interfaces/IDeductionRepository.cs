using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IDeductionRepository
{
    Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Deduction?> GetDeductionAsync(Guid deductionId, CancellationToken cancellationToken = default);

    IQueryable<Deduction> GetDeductions();

    Task<bool> DeductionExistsAsync(Guid employeeId, string reason, int deductionMonth, int deductionYear, CancellationToken cancellationToken = default);

    Task AddDeductionAsync(Deduction deduction, CancellationToken cancellationToken = default);

    void UpdateDeduction(Deduction deduction);

    void DeleteDeduction(Deduction deduction);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
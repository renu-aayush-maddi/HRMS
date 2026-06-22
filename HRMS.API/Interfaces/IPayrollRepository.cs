using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IPayrollRepository
{
    Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Payroll?> GetPayrollByIdAsync(Guid payrollId, CancellationToken cancellationToken = default);

    Task<Payroll?> GetPayrollAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default);

    IQueryable<Payroll> GetPayrolls();

    Task AddPayrollAsync(Payroll payroll, CancellationToken cancellationToken = default);

    void UpdatePayroll(Payroll payroll);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<int> GetPresentDaysAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default);

    Task<int> GetWorkingDaysAsync(int month, int year, CancellationToken cancellationToken = default);

    Task<int> GetApprovedPaidLeaveDaysAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default);

    Task<int> GetApprovedLopLeaveDaysAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default);

    Task<EmployeeSalary?> GetActiveEmployeeSalaryAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<decimal> GetApprovedBonusAmountAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default);

    Task<decimal> GetApprovedDeductionAmountAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default);

    Task<List<Employee>> GetActiveEmployeesAsync(CancellationToken cancellationToken = default);

    Task<List<Bonuse>> GetApprovedBonusesAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default);

    Task<List<Deduction>> GetApprovedDeductionsAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default);
}
using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IDeductionRepository
{
    Employee? GetEmployee(Guid employeeId);

    Employee? GetEmployeeByUserId(Guid userId);

    Deduction? GetDeduction(Guid deductionId);

    List<Deduction> GetAllDeductions();

    List<Deduction> GetEmployeeDeductions(Guid employeeId);

    void AddDeduction(Deduction deduction);

    void UpdateDeduction(Deduction deduction);

    void SaveChanges();
}
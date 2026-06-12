using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IPayrollRepository
{
    Employee? GetEmployee(Guid employeeId);

    List<Payroll> GetAllPayrolls();

    List<Payroll> GetEmployeePayrolls(Guid employeeId);

    void AddPayroll(Payroll payroll);

    void SaveChanges();


    int GetPresentDays(Guid employeeId,int month,int year);

    int GetWorkingDays(int month,int year);


    int GetApprovedPaidLeaveDays(Guid employeeId,int month,int year);

    int GetApprovedLopLeaveDays(Guid employeeId,int month,int year);

    Payroll? GetPayrollById(Guid payrollId);

    void UpdatePayroll(Payroll payroll);

    Employee? GetEmployeeByUserId(Guid userId);

    EmployeeSalary? GetActiveEmployeeSalary(Guid employeeId);

    decimal GetApprovedBonusAmount(Guid employeeId,int month,int year);

    decimal GetApprovedDeductionAmount(Guid employeeId,int month,int year);

    List<Employee> GetActiveEmployees();

    Payroll? GetPayroll(Guid employeeId,int month,int year);


    List<Bonuse> GetApprovedBonuses(Guid employeeId,int month,int year);

    List<Deduction> GetApprovedDeductions(Guid employeeId,int month,int year);

}
using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IEmployeeSalaryRepository
{
    Employee? GetEmployee(Guid employeeId);

    SalaryStructure? GetSalaryStructure(
        Guid salaryStructureId);

    EmployeeSalary? GetActiveSalary(
        Guid employeeId);

    List<EmployeeSalary> GetAll();

    void Add(EmployeeSalary employeeSalary);

    void Update(EmployeeSalary employeeSalary);

    void SaveChanges();

    List<EmployeeSalary> GetSalaryHistory(Guid employeeId);
}
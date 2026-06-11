using HRMS.API.Models.DTOs.EmployeeSalary;

namespace HRMS.API.Interfaces;

public interface IEmployeeSalaryService
{
    void AssignSalary(
        AssignEmployeeSalaryDto dto);

    EmployeeSalaryResponseDto
        GetActiveSalary(
            Guid employeeId);

    List<EmployeeSalaryResponseDto>
        GetAll();

        List<SalaryHistoryResponseDto>
    GetSalaryHistory(
        Guid employeeId);
}
using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.EmployeeSalary;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

public class EmployeeSalaryService: IEmployeeSalaryService
{
    private readonly IEmployeeSalaryRepository employeeSalaryRepository;

    public EmployeeSalaryService(
        IEmployeeSalaryRepository employeeSalaryRepository)
    {
        this.employeeSalaryRepository =
            employeeSalaryRepository;
    }

    public void AssignSalary(
        AssignEmployeeSalaryDto dto)
    {
        var employee =
            employeeSalaryRepository
            .GetEmployee(dto.EmployeeId);

        if (employee == null)
        {
            throw new NotFoundException(
                "Employee not found");
        }

        var structure =
            employeeSalaryRepository
            .GetSalaryStructure(
                dto.SalaryStructureId);

        if (structure == null)
        {
            throw new NotFoundException(
                "Salary structure not found");
        }

        if (dto.AnnualCtc <= 0)
        {
            throw new BusinessException(
                "Annual CTC must be greater than zero");
        }

        var currentSalary =
            employeeSalaryRepository
            .GetActiveSalary(
                dto.EmployeeId);

        if (currentSalary != null)
        {
            currentSalary.IsActive = false;

            employeeSalaryRepository
                .Update(currentSalary);
        }

        EmployeeSalary employeeSalary =
            new EmployeeSalary
            {
                Id = Guid.NewGuid(),

                EmployeeId =
                    dto.EmployeeId,

                SalaryStructureId =
                    dto.SalaryStructureId,

                AnnualCtc =
                    dto.AnnualCtc,

                EffectiveFrom =
                    dto.EffectiveFrom,

                IsActive = true
            };

        employeeSalaryRepository
            .Add(employeeSalary);

        employeeSalaryRepository
            .SaveChanges();
    }

    public EmployeeSalaryResponseDto
        GetActiveSalary(Guid employeeId)
    {
        var salary =
            employeeSalaryRepository
            .GetActiveSalary(employeeId);

        if (salary == null)
        {
            throw new NotFoundException(
                "Active salary not found");
        }

        return new EmployeeSalaryResponseDto
        {
            Id = salary.Id,

            EmployeeName =
                salary.Employee.FirstName +
                " " +
                salary.Employee.LastName,

            SalaryStructureName =
                salary.SalaryStructure.Name,

            AnnualCtc =
                salary.AnnualCtc,

            EffectiveFrom =
                salary.EffectiveFrom,

            IsActive =
                salary.IsActive
        };
    }

    public List<EmployeeSalaryResponseDto>
        GetAll()
    {
        return employeeSalaryRepository
            .GetAll()
            .Select(s => new EmployeeSalaryResponseDto
            {
                Id = s.Id,

                EmployeeName =
                    s.Employee.FirstName +
                    " " +
                    s.Employee.LastName,

                SalaryStructureName =
                    s.SalaryStructure.Name,

                AnnualCtc =
                    s.AnnualCtc,

                EffectiveFrom =
                    s.EffectiveFrom,

                IsActive =
                    s.IsActive
            })
            .ToList();
    }

    public List<SalaryHistoryResponseDto>
    GetSalaryHistory(
        Guid employeeId)
    {
        var employee =
            employeeSalaryRepository
            .GetEmployee(employeeId);

        if (employee == null)
        {
            throw new NotFoundException(
                "Employee not found");
        }

        return employeeSalaryRepository
            .GetSalaryHistory(employeeId)
            .Select(x =>
                new SalaryHistoryResponseDto
                {
                    Id = x.Id,

                    EmployeeName =
                        x.Employee.FirstName +
                        " " +
                        x.Employee.LastName,

                    SalaryStructureName =
                        x.SalaryStructure.Name,

                    AnnualCtc =
                        x.AnnualCtc,

                    EffectiveFrom =
                        x.EffectiveFrom,

                    IsActive =
                        x.IsActive
                })
            .ToList();
    }
}
using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Employee;

namespace HRMS.API.Validators;

public class EmployeeValidator
{
    private readonly IEmployeeRepository employeeRepository;

    private static readonly HashSet<string> ValidStatuses =
    [
        "Active",
        "Probation",
        "OnLeave",
        "NoticePeriod",
        "Resigned",
        "Terminated",
        "Inactive"
    ];

    public EmployeeValidator(IEmployeeRepository employeeRepository)
    {
        this.employeeRepository = employeeRepository;
    }

    public async Task ValidateCreateAsync(AddEmployeeDto dto)
    {
        if (await employeeRepository.EmployeeExistsAsync(dto.Email))
        {
            throw new BusinessException("Employee already exists");
        }

        if (!await employeeRepository.DepartmentExistsAsync(dto.DepartmentId))
        {
            throw new BusinessException("Department not found");
        }

        var role = await employeeRepository.GetRoleByNameAsync(dto.Role);

        if (role == null)
        {
            throw new BusinessException("Invalid role");
        }

        if (dto.Salary <= 0)
        {
            throw new BusinessException("Salary must be greater than zero");
        }
    }

    public Task ValidateUpdateAsync(UpdateEmployeeDto dto)
    {
        if (dto.Salary <= 0)
        {
            throw new BusinessException("Salary must be greater than zero");
        }

        return Task.CompletedTask;
    }

    public Task ValidateStatusAsync(string status)
    {
        if (!ValidStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
        {
            throw new BusinessException("Invalid employment status");
        }

        return Task.CompletedTask;
    }
}
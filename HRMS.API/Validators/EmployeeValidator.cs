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

    public EmployeeValidator(
        IEmployeeRepository employeeRepository)
    {
        this.employeeRepository = employeeRepository;
    }

    public async Task ValidateCreateAsync(
        AddEmployeeDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName))
        {
            throw new BusinessException(
                "First name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.LastName))
        {
            throw new BusinessException(
                "Last name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new BusinessException(
                "Email is required.");
        }

        if (await employeeRepository.EmployeeExistsAsync(
                dto.Email,
                cancellationToken))
        {
            throw new BusinessException(
                "Employee already exists.");
        }

        var departmentExists =
            await employeeRepository.DepartmentExistsAsync(
                dto.DepartmentId,
                cancellationToken);

        if (!departmentExists)
        {
            throw new BusinessException(
                "Department not found.");
        }

        var role =
            await employeeRepository.GetRoleByNameAsync(
                dto.Role,
                cancellationToken);

        if (role == null)
        {
            throw new BusinessException(
                "Invalid role.");
        }

        if (dto.ManagerId.HasValue)
        {
            var managerExists =
                await employeeRepository.ManagerExistsAsync(
                    dto.ManagerId.Value,
                    cancellationToken);

            if (!managerExists)
            {
                throw new BusinessException(
                    "Manager not found.");
            }
        }

        if (dto.Salary <= 0)
        {
            throw new BusinessException(
                "Salary must be greater than zero.");
        }
    }

    public async Task ValidateUpdateAsync(
        UpdateEmployeeDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName))
        {
            throw new BusinessException(
                "First name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.LastName))
        {
            throw new BusinessException(
                "Last name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Designation))
        {
            throw new BusinessException(
                "Designation is required.");
        }

        if (dto.Salary <= 0)
        {
            throw new BusinessException(
                "Salary must be greater than zero.");
        }

        await Task.CompletedTask;
    }

    public Task ValidateStatusAsync(
        string status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            throw new BusinessException(
                "Status is required.");
        }

        if (!ValidStatuses.Contains(
                status,
                StringComparer.OrdinalIgnoreCase))
        {
            throw new BusinessException(
                "Invalid employment status.");
        }

        return Task.CompletedTask;
    }

    public async Task ValidateImportAsync(
        AddEmployeeDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateCreateAsync(
            dto,
            cancellationToken);
    }
}
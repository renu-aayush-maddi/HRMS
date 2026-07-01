using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.LeaveBalance;
using HRMS.API.Models.Entities;
using HRMS.API.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HRMS.API.Services;

public class LeaveBalanceService : ILeaveBalanceService
{
    private readonly ILeaveBalanceRepository repository;
    private readonly IEmployeeAccessResolver accessResolver;
    private readonly LeaveBalanceValidator validator;

    public LeaveBalanceService(
        ILeaveBalanceRepository repository,
        IEmployeeAccessResolver accessResolver,
        LeaveBalanceValidator validator)
    {
        this.repository = repository;
        this.accessResolver = accessResolver;
        this.validator = validator;
    }

    public async Task AllocateAsync(AllocateLeaveBalanceDto dto, CancellationToken cancellationToken = default)
    {
        var employeeId = await accessResolver.ResolveEmployeeIdAsync(dto.EmployeeId, cancellationToken);
        await accessResolver.ValidateEmployeeOwnershipAsync(employeeId, cancellationToken);

        dto.EmployeeId = employeeId;

        await validator.ValidateAllocationAsync(dto);

        var employee =
            await repository.GetEmployeeAsync(
                dto.EmployeeId);

        if (employee == null)
        {
            throw new NotFoundException("Employee not found");
        }

        var leaveType =
            await repository.GetLeaveTypeAsync(
                dto.LeaveTypeId);

        if (leaveType == null)
        {
            throw new NotFoundException(
                "Leave type not found");
        }

        var existing =
            await repository.GetBalanceAsync(
                dto.EmployeeId,
                dto.LeaveTypeId);

        if (existing != null)
        {
            existing.AllocatedDays += dto.AllocatedDays;
            existing.RemainingDays += dto.AllocatedDays;

            repository.UpdateBalance(existing);

            await repository.SaveChangesAsync();

            return;
        }

        var balance = new EmployeeLeaveBalance
        {
            Id = Guid.NewGuid(),
            EmployeeId = dto.EmployeeId,
            LeaveTypeId = dto.LeaveTypeId,
            AllocatedDays = dto.AllocatedDays,
            UsedDays = 0,
            RemainingDays = dto.AllocatedDays
        };

        await repository.AddBalanceAsync(balance);

        await repository.SaveChangesAsync();
    }

    public async Task<List<LeaveBalanceResponseDto>> GetAllBalancesAsync(CancellationToken cancellationToken = default)
    {
        var balances =
            await repository.GetAllBalancesAsync();

        return balances
            .Select(x => new LeaveBalanceResponseDto
            {
                Id = x.Id,
                EmployeeName =
                    $"{x.Employee!.FirstName} {x.Employee.LastName}",
                LeaveType = x.LeaveType!.Name,
                AllocatedDays = x.AllocatedDays,
                UsedDays = x.UsedDays ?? 0,
                RemainingDays = x.RemainingDays
            })
            .ToList();
    }

    public async Task<List<LeaveBalanceResponseDto>> GetEmployeeBalancesAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var resolvedEmployeeId = await accessResolver.ResolveEmployeeIdAsync(employeeId, cancellationToken);
        await accessResolver.ValidateEmployeeOwnershipAsync(resolvedEmployeeId, cancellationToken);

        var balances =
            await repository.GetEmployeeBalancesAsync(resolvedEmployeeId);

        return balances
            .Select(x => new LeaveBalanceResponseDto
            {
                Id = x.Id,
                EmployeeName =
                    $"{x.Employee!.FirstName} {x.Employee.LastName}",
                LeaveType = x.LeaveType!.Name,
                AllocatedDays = x.AllocatedDays,
                UsedDays = x.UsedDays ?? 0,
                RemainingDays = x.RemainingDays
            })
            .ToList();
    }

    public async Task AllocateDefaultBalancesAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var employee =
            await repository.GetEmployeeAsync(
                employeeId);

        if (employee == null)
        {
            throw new NotFoundException("Employee not found");
        }

        var leaveTypes =
            await repository.GetActiveLeaveTypesAsync();

        foreach (var leaveType in leaveTypes)
        {
            var existing =
                await repository.GetBalanceAsync(
                    employeeId,
                    leaveType.Id);

            if (existing != null)
            {
                continue;
            }

            var balance = new EmployeeLeaveBalance
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                LeaveTypeId = leaveType.Id,
                AllocatedDays = leaveType.AnnualAllocation,
                UsedDays = 0,
                RemainingDays = leaveType.AnnualAllocation
            };

            await repository.AddBalanceAsync(balance);
        }

        await repository.SaveChangesAsync();
    }
}
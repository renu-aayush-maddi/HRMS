using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.LeaveBalance;
using HRMS.API.Models.Entities;
using HRMS.API.Exceptions;

namespace HRMS.API.Services;

public class LeaveBalanceService: ILeaveBalanceService
{
    private readonly ILeaveBalanceRepository repository;
    

    public LeaveBalanceService(ILeaveBalanceRepository repository)
    {
        this.repository = repository;
    }

    public void Allocate(AllocateLeaveBalanceDto dto)
    {
        var employee =
            repository.GetEmployee(
                dto.EmployeeId);

        if(employee == null)
        {
            throw new NotFoundException("Employee not found");
        }

        var leaveType =
            repository.GetLeaveType(
                dto.LeaveTypeId);

        if(leaveType == null)
        {
            throw new NotFoundException("Leave type not found");
        }

        var existing =
            repository.GetBalance(
                dto.EmployeeId,
                dto.LeaveTypeId);

        if(existing != null)
        {
            throw new BusinessException("Balance already exists");  
        }

        EmployeeLeaveBalance balance =
            new EmployeeLeaveBalance
            {
                Id = Guid.NewGuid(),

                EmployeeId =
                    dto.EmployeeId,

                LeaveTypeId =
                    dto.LeaveTypeId,

                AllocatedDays =
                    dto.AllocatedDays,

                UsedDays = 0,

                RemainingDays =
                    dto.AllocatedDays
            };

        repository.AddBalance(balance);

        repository.SaveChanges();
    }

    public List<LeaveBalanceResponseDto>
        GetAllBalances()
    {
        return repository
            .GetAllBalances()
            .Select(x =>
                new LeaveBalanceResponseDto
                {
                    Id = x.Id,

                    EmployeeName =
                        x.Employee!.FirstName
                        + " "
                        + x.Employee.LastName,

                    LeaveType =
                        x.LeaveType!.Name,

                    AllocatedDays =
                        x.AllocatedDays,

                    UsedDays =
                        x.UsedDays ?? 0,

                    RemainingDays =
                        x.RemainingDays
                })
            .ToList();
    }

    public List<LeaveBalanceResponseDto> GetEmployeeBalances(Guid employeeId)
    {
        return repository
            .GetEmployeeBalances(
                employeeId)
            .Select(x =>
                new LeaveBalanceResponseDto
                {
                    Id = x.Id,

                    EmployeeName =
                        x.Employee!.FirstName
                        + " "
                        + x.Employee.LastName,

                    LeaveType =
                        x.LeaveType!.Name,

                    AllocatedDays =
                        x.AllocatedDays,

                    UsedDays =
                        x.UsedDays ?? 0,

                    RemainingDays =
                        x.RemainingDays
                })
            .ToList();
    }

    public void AllocateDefaultBalances(
        Guid employeeId)
    {
        var employee =
            repository.GetEmployee(employeeId);

        if(employee == null)
        {
            throw new NotFoundException("Employee not found");
        }

        var leaveTypes =
            repository.GetActiveLeaveTypes();

        foreach(var leaveType in leaveTypes)
        {
            bool exists =
                repository.GetBalance(
                    employeeId,
                    leaveType.Id) != null;

            if(exists)
            {
                continue;
            }

            EmployeeLeaveBalance balance =
                new EmployeeLeaveBalance
                {
                    Id = Guid.NewGuid(),

                    EmployeeId = employeeId,

                    LeaveTypeId = leaveType.Id,

                    AllocatedDays =
                        leaveType.AnnualAllocation,

                    UsedDays = 0,

                    RemainingDays =
                        leaveType.AnnualAllocation
                };

            repository.AddBalance(balance);
        }

        repository.SaveChanges();
    }
}
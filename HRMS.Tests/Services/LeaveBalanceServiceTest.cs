using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.LeaveBalance;
using HRMS.API.Exceptions;
using HRMS.API.Validators;

namespace HRMS.Tests.Services;

[TestFixture]
public class LeaveBalanceServiceTests
{
private Mock<ILeaveBalanceRepository>
repository;


private LeaveBalanceValidator
    validator;

private LeaveBalanceService
    service;

[SetUp]
public void Setup()
{
    repository =
        new Mock<ILeaveBalanceRepository>();

    validator =
        new LeaveBalanceValidator();

    service =
        new LeaveBalanceService(
            repository.Object,
            validator);
}

[Test]
public async Task AllocateAsync_ShouldAddBalance_WhenValid()
{
    var employeeId =
        Guid.NewGuid();

    var leaveTypeId =
        Guid.NewGuid();

    repository
        .Setup(x =>
            x.GetEmployeeAsync(
                employeeId))
        .ReturnsAsync(
            new Employee());

    repository
        .Setup(x =>
            x.GetLeaveTypeAsync(
                leaveTypeId))
        .ReturnsAsync(
            new LeaveType());

    repository
        .Setup(x =>
            x.GetBalanceAsync(
                employeeId,
                leaveTypeId))
        .ReturnsAsync(
            (EmployeeLeaveBalance?)null);

    await service
        .AllocateAsync(
            new AllocateLeaveBalanceDto
            {
                EmployeeId =
                    employeeId,

                LeaveTypeId =
                    leaveTypeId,

                AllocatedDays =
                    10
            });

    repository.Verify(
        x => x.AddBalanceAsync(
            It.IsAny<EmployeeLeaveBalance>()),
        Times.Once);

    repository.Verify(
        x => x.SaveChangesAsync(),
        Times.Once);
}

[Test]
public async Task AllocateAsync_ShouldUpdateExistingBalance_WhenBalanceExists()
{
    var employeeId =
        Guid.NewGuid();

    var leaveTypeId =
        Guid.NewGuid();

    var balance =
        new EmployeeLeaveBalance
        {
            AllocatedDays = 10,
            RemainingDays = 10
        };

    repository
        .Setup(x =>
            x.GetEmployeeAsync(
                employeeId))
        .ReturnsAsync(
            new Employee());

    repository
        .Setup(x =>
            x.GetLeaveTypeAsync(
                leaveTypeId))
        .ReturnsAsync(
            new LeaveType());

    repository
        .Setup(x =>
            x.GetBalanceAsync(
                employeeId,
                leaveTypeId))
        .ReturnsAsync(
            balance);

    await service
        .AllocateAsync(
            new AllocateLeaveBalanceDto
            {
                EmployeeId =
                    employeeId,

                LeaveTypeId =
                    leaveTypeId,

                AllocatedDays =
                    5
            });

    Assert.That(
        balance.AllocatedDays,
        Is.EqualTo(15));

    Assert.That(
        balance.RemainingDays,
        Is.EqualTo(15));

    repository.Verify(
        x => x.UpdateBalance(
            balance),
        Times.Once);

    repository.Verify(
        x => x.SaveChangesAsync(),
        Times.Once);
}

[Test]
public void AllocateAsync_ShouldThrowValidationException_WhenAllocationInvalid()
{
    Assert.ThrowsAsync<
        ValidationException>(
        async () =>
            await service
                .AllocateAsync(
                    new AllocateLeaveBalanceDto
                    {
                        EmployeeId =
                            Guid.NewGuid(),

                        LeaveTypeId =
                            Guid.NewGuid(),

                        AllocatedDays =
                            0
                    }));
}

[Test]
public void AllocateAsync_ShouldThrowNotFoundException_WhenEmployeeNotFound()
{
    repository
        .Setup(x =>
            x.GetEmployeeAsync(
                It.IsAny<Guid>()))
        .ReturnsAsync(
            (Employee?)null);

    Assert.ThrowsAsync<
        NotFoundException>(
        async () =>
            await service
                .AllocateAsync(
                    new AllocateLeaveBalanceDto
                    {
                        EmployeeId =
                            Guid.NewGuid(),

                        LeaveTypeId =
                            Guid.NewGuid(),

                        AllocatedDays =
                            10
                    }));
}

[Test]
public void AllocateAsync_ShouldThrowNotFoundException_WhenLeaveTypeNotFound()
{
    repository
        .Setup(x =>
            x.GetEmployeeAsync(
                It.IsAny<Guid>()))
        .ReturnsAsync(
            new Employee());

    repository
        .Setup(x =>
            x.GetLeaveTypeAsync(
                It.IsAny<Guid>()))
        .ReturnsAsync(
            (LeaveType?)null);

    Assert.ThrowsAsync<
        NotFoundException>(
        async () =>
            await service
                .AllocateAsync(
                    new AllocateLeaveBalanceDto
                    {
                        EmployeeId =
                            Guid.NewGuid(),

                        LeaveTypeId =
                            Guid.NewGuid(),

                        AllocatedDays =
                            10
                    }));
}

[Test]
public async Task GetAllBalancesAsync_ShouldReturnMappedDtos()
{
    repository
        .Setup(x =>
            x.GetAllBalancesAsync())
        .ReturnsAsync(
            new List<EmployeeLeaveBalance>
            {
                new EmployeeLeaveBalance
                {
                    Id = Guid.NewGuid(),

                    AllocatedDays = 20,

                    UsedDays = 5,

                    RemainingDays = 15,

                    Employee =
                        new Employee
                        {
                            FirstName = "John",
                            LastName = "Doe"
                        },

                    LeaveType =
                        new LeaveType
                        {
                            Name = "Casual Leave"
                        }
                }
            });

    var result =
        await service
            .GetAllBalancesAsync();

    Assert.That(
        result.Count,
        Is.EqualTo(1));

    Assert.That(
        result[0].EmployeeName,
        Is.EqualTo(
            "John Doe"));

    Assert.That(
        result[0].LeaveType,
        Is.EqualTo(
            "Casual Leave"));
}

[Test]
public async Task GetEmployeeBalancesAsync_ShouldReturnMappedDtos()
{
    repository
        .Setup(x =>
            x.GetEmployeeBalancesAsync(
                It.IsAny<Guid>()))
        .ReturnsAsync(
            new List<EmployeeLeaveBalance>
            {
                new EmployeeLeaveBalance
                {
                    Id = Guid.NewGuid(),

                    AllocatedDays = 20,

                    UsedDays = 5,

                    RemainingDays = 15,

                    Employee =
                        new Employee
                        {
                            FirstName = "John",
                            LastName = "Doe"
                        },

                    LeaveType =
                        new LeaveType
                        {
                            Name = "Sick Leave"
                        }
                }
            });

    var result =
        await service
            .GetEmployeeBalancesAsync(
                Guid.NewGuid());

    Assert.That(
        result.Count,
        Is.EqualTo(1));

    Assert.That(
        result[0].LeaveType,
        Is.EqualTo(
            "Sick Leave"));
}

[Test]
public async Task AllocateDefaultBalancesAsync_ShouldAllocateBalances_WhenValid()
{
    var employeeId =
        Guid.NewGuid();

    repository
        .Setup(x =>
            x.GetEmployeeAsync(
                employeeId))
        .ReturnsAsync(
            new Employee());

    repository
        .Setup(x =>
            x.GetActiveLeaveTypesAsync())
        .ReturnsAsync(
            new List<LeaveType>
            {
                new LeaveType
                {
                    Id = Guid.NewGuid(),
                    AnnualAllocation = 12
                },
                new LeaveType
                {
                    Id = Guid.NewGuid(),
                    AnnualAllocation = 15
                }
            });

    repository
        .Setup(x =>
            x.GetBalanceAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>()))
        .ReturnsAsync(
            (EmployeeLeaveBalance?)null);

    await service
        .AllocateDefaultBalancesAsync(
            employeeId);

    repository.Verify(
        x => x.AddBalanceAsync(
            It.IsAny<EmployeeLeaveBalance>()),
        Times.Exactly(2));

    repository.Verify(
        x => x.SaveChangesAsync(),
        Times.Once);
}

[Test]
public async Task AllocateDefaultBalancesAsync_ShouldSkipExistingBalances()
{
    var employeeId =
        Guid.NewGuid();

    repository
        .Setup(x =>
            x.GetEmployeeAsync(
                employeeId))
        .ReturnsAsync(
            new Employee());

    repository
        .Setup(x =>
            x.GetActiveLeaveTypesAsync())
        .ReturnsAsync(
            new List<LeaveType>
            {
                new LeaveType
                {
                    Id = Guid.NewGuid(),
                    AnnualAllocation = 12
                }
            });

    repository
        .Setup(x =>
            x.GetBalanceAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>()))
        .ReturnsAsync(
            new EmployeeLeaveBalance());

    await service
        .AllocateDefaultBalancesAsync(
            employeeId);

    repository.Verify(
        x => x.AddBalanceAsync(
            It.IsAny<EmployeeLeaveBalance>()),
        Times.Never);

    repository.Verify(
        x => x.SaveChangesAsync(),
        Times.Once);
}

[Test]
public void AllocateDefaultBalancesAsync_ShouldThrowNotFoundException_WhenEmployeeNotFound()
{
    repository
        .Setup(x =>
            x.GetEmployeeAsync(
                It.IsAny<Guid>()))
        .ReturnsAsync(
            (Employee?)null);

    Assert.ThrowsAsync<
        NotFoundException>(
        async () =>
            await service
                .AllocateDefaultBalancesAsync(
                    Guid.NewGuid()));
}


}

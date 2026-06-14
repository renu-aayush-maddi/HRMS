using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Exceptions;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.Leave;
using HRMS.API.Validators;

namespace HRMS.Tests.Services;

[TestFixture]
public class LeaveServiceTests
{
private Mock<ILeaveRepository>
repository;


private Mock<INotificationService>
    notificationService;

private LeaveValidator
    validator;

private LeaveService
    service;

[SetUp]
public void Setup()
{
    repository =
        new Mock<ILeaveRepository>();

    notificationService =
        new Mock<INotificationService>();

    validator =
        new LeaveValidator();

    service =
        new LeaveService(
            repository.Object,
            notificationService.Object,
            validator);
}

[Test]
public async Task ApplyLeaveAsync_ShouldApplyLeave_WhenValid()
{
    var employeeId =
        Guid.NewGuid();

    var leaveTypeId =
        Guid.NewGuid();

    var userId =
        Guid.NewGuid();

    repository
        .Setup(x =>
            x.GetEmployeeByUserIdAsync(
                userId))
        .ReturnsAsync(
            new Employee
            {
                Id = employeeId
            });

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
            x.GetLeaveBalanceAsync(
                employeeId,
                leaveTypeId))
        .ReturnsAsync(
            new EmployeeLeaveBalance
            {
                RemainingDays = 20
            });

    repository
        .Setup(x =>
            x.HasOverlappingLeaveAsync(
                employeeId,
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>()))
        .ReturnsAsync(false);

    await service
        .ApplyLeaveAsync(
            new ApplyLeaveDto
            {
                LeaveTypeId =
                    leaveTypeId,

                FromDate =
                    DateOnly.FromDateTime(
                        DateTime.Today.AddDays(1)),

                ToDate =
                    DateOnly.FromDateTime(
                        DateTime.Today.AddDays(3)),

                Reason =
                    "Vacation"
            },
            userId,
            "Employee");

    repository.Verify(
        x => x.AddLeaveAsync(
            It.IsAny<LeaveRequest>()),
        Times.Once);

    repository.Verify(
        x => x.SaveChangesAsync(),
        Times.Once);
}

[Test]
public void ApplyLeaveAsync_ShouldThrowNotFoundException_WhenLoggedInEmployeeNotFound()
{
    repository
        .Setup(x =>
            x.GetEmployeeByUserIdAsync(
                It.IsAny<Guid>()))
        .ReturnsAsync(
            (Employee?)null);

    Assert.ThrowsAsync<
        NotFoundException>(
        async () =>
            await service
                .ApplyLeaveAsync(
                    new ApplyLeaveDto
                    {
                        LeaveTypeId =
                            Guid.NewGuid(),

                        FromDate =
                            DateOnly.FromDateTime(
                                DateTime.Today.AddDays(1)),

                        ToDate =
                            DateOnly.FromDateTime(
                                DateTime.Today.AddDays(2)),

                        Reason =
                            "Vacation"
                    },
                    Guid.NewGuid(),
                    "Employee"));
}

[Test]
public void ApplyLeaveAsync_ShouldThrowBusinessException_WhenEmployeeIdMissingForAdmin()
{
    Assert.ThrowsAsync<
        BusinessException>(
        async () =>
            await service
                .ApplyLeaveAsync(
                    new ApplyLeaveDto
                    {
                        EmployeeId = null,

                        LeaveTypeId =
                            Guid.NewGuid(),

                        FromDate =
                            DateOnly.FromDateTime(
                                DateTime.Today.AddDays(1)),

                        ToDate =
                            DateOnly.FromDateTime(
                                DateTime.Today.AddDays(2)),

                        Reason =
                            "Vacation"
                    },
                    Guid.NewGuid(),
                    "Admin"));
}

[Test]
public void ApplyLeaveAsync_ShouldThrowNotFoundException_WhenEmployeeNotFound()
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
                .ApplyLeaveAsync(
                    new ApplyLeaveDto
                    {
                        EmployeeId =
                            Guid.NewGuid(),

                        LeaveTypeId =
                            Guid.NewGuid(),

                        FromDate =
                            DateOnly.FromDateTime(
                                DateTime.Today.AddDays(1)),

                        ToDate =
                            DateOnly.FromDateTime(
                                DateTime.Today.AddDays(2)),

                        Reason =
                            "Vacation"
                    },
                    Guid.NewGuid(),
                    "Admin"));
}

[Test]
public void ApplyLeaveAsync_ShouldThrowNotFoundException_WhenLeaveTypeNotFound()
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
                .ApplyLeaveAsync(
                    new ApplyLeaveDto
                    {
                        EmployeeId =
                            Guid.NewGuid(),

                        LeaveTypeId =
                            Guid.NewGuid(),

                        FromDate =
                            DateOnly.FromDateTime(
                                DateTime.Today.AddDays(1)),

                        ToDate =
                            DateOnly.FromDateTime(
                                DateTime.Today.AddDays(2)),

                        Reason =
                            "Vacation"
                    },
                    Guid.NewGuid(),
                    "Admin"));
}

[Test]
public void ApplyLeaveAsync_ShouldThrowNotFoundException_WhenLeaveBalanceNotAllocated()
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
            new LeaveType());

    repository
        .Setup(x =>
            x.GetLeaveBalanceAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>()))
        .ReturnsAsync(
            (EmployeeLeaveBalance?)null);

    Assert.ThrowsAsync<
        NotFoundException>(
        async () =>
            await service
                .ApplyLeaveAsync(
                    new ApplyLeaveDto
                    {
                        EmployeeId =
                            Guid.NewGuid(),

                        LeaveTypeId =
                            Guid.NewGuid(),

                        FromDate =
                            DateOnly.FromDateTime(
                                DateTime.Today.AddDays(1)),

                        ToDate =
                            DateOnly.FromDateTime(
                                DateTime.Today.AddDays(2)),

                        Reason =
                            "Vacation"
                    },
                    Guid.NewGuid(),
                    "Admin"));
}

[Test]
public void ApplyLeaveAsync_ShouldThrowBusinessException_WhenInsufficientBalance()
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
            new LeaveType());

    repository
        .Setup(x =>
            x.GetLeaveBalanceAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>()))
        .ReturnsAsync(
            new EmployeeLeaveBalance
            {
                RemainingDays = 0
            });

    Assert.ThrowsAsync<
        BusinessException>(
        async () =>
            await service
                .ApplyLeaveAsync(
                    new ApplyLeaveDto
                    {
                        EmployeeId =
                            Guid.NewGuid(),

                        LeaveTypeId =
                            Guid.NewGuid(),

                        FromDate =
                            DateOnly.FromDateTime(
                                DateTime.Today.AddDays(1)),

                        ToDate =
                            DateOnly.FromDateTime(
                                DateTime.Today.AddDays(2)),

                        Reason =
                            "Vacation"
                    },
                    Guid.NewGuid(),
                    "Admin"));
}


}

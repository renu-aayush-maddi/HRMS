using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Exceptions;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.Attendance;
using HRMS.API.Models.DTOs.Common;
using HRMS.API.Validators;

namespace HRMS.Tests.Services;

[TestFixture]
public class AttendanceServiceTests
{
private Mock<IAttendanceRepository>
repository;


private AttendanceValidator
    validator;

private AttendanceService
    service;

[SetUp]
public void Setup()
{
    repository =
        new Mock<IAttendanceRepository>();

    validator =
        new AttendanceValidator();

    service =
        new AttendanceService(
            repository.Object,
            validator);
}

[Test]
public async Task CheckInAsync_ShouldCheckIn_WhenValid()
{
    var userId =
        Guid.NewGuid();

    var employee =
        new Employee
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetEmployeeByUserIdAsync(
                userId))
        .ReturnsAsync(employee);

    repository
        .Setup(x =>
            x.GetTodayAttendanceAsync(
                employee.Id))
        .ReturnsAsync(
            (AttendanceLog?)null);

    await service
        .CheckInAsync(userId);

    repository.Verify(
        x => x.AddAttendanceAsync(
            It.IsAny<AttendanceLog>()),
        Times.Once);

    repository.Verify(
        x => x.SaveChangesAsync(),
        Times.Once);
}

[Test]
public void CheckInAsync_ShouldThrowNotFoundException_WhenEmployeeNotFound()
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
                .CheckInAsync(
                    Guid.NewGuid()));
}

[Test]
public void CheckInAsync_ShouldThrowBusinessException_WhenAlreadyCheckedIn()
{
    var employee =
        new Employee
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetEmployeeByUserIdAsync(
                It.IsAny<Guid>()))
        .ReturnsAsync(employee);

    repository
        .Setup(x =>
            x.GetTodayAttendanceAsync(
                employee.Id))
        .ReturnsAsync(
            new AttendanceLog());

    Assert.ThrowsAsync<
        BusinessException>(
        async () =>
            await service
                .CheckInAsync(
                    Guid.NewGuid()));
}

[Test]
public async Task CheckOutAsync_ShouldCheckOut_WhenValid()
{
    var employee =
        new Employee
        {
            Id = Guid.NewGuid()
        };

    var attendance =
        new AttendanceLog
        {
            CheckIn =
                DateTime.UtcNow
        };

    repository
        .Setup(x =>
            x.GetEmployeeByUserIdAsync(
                It.IsAny<Guid>()))
        .ReturnsAsync(employee);

    repository
        .Setup(x =>
            x.GetTodayAttendanceAsync(
                employee.Id))
        .ReturnsAsync(attendance);

    await service
        .CheckOutAsync(
            Guid.NewGuid());

    repository.Verify(
        x => x.UpdateAttendance(
            attendance),
        Times.Once);

    repository.Verify(
        x => x.SaveChangesAsync(),
        Times.Once);

    Assert.That(
        attendance.CheckOut,
        Is.Not.Null);
}

[Test]
public void CheckOutAsync_ShouldThrowNotFoundException_WhenEmployeeNotFound()
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
                .CheckOutAsync(
                    Guid.NewGuid()));
}

[Test]
public void CheckOutAsync_ShouldThrowNotFoundException_WhenCheckInNotFound()
{
    var employee =
        new Employee
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetEmployeeByUserIdAsync(
                It.IsAny<Guid>()))
        .ReturnsAsync(employee);

    repository
        .Setup(x =>
            x.GetTodayAttendanceAsync(
                employee.Id))
        .ReturnsAsync(
            (AttendanceLog?)null);

    Assert.ThrowsAsync<
        NotFoundException>(
        async () =>
            await service
                .CheckOutAsync(
                    Guid.NewGuid()));
}

[Test]
public void CheckOutAsync_ShouldThrowBusinessException_WhenAlreadyCheckedOut()
{
    var employee =
        new Employee
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetEmployeeByUserIdAsync(
                It.IsAny<Guid>()))
        .ReturnsAsync(employee);

    repository
        .Setup(x =>
            x.GetTodayAttendanceAsync(
                employee.Id))
        .ReturnsAsync(
            new AttendanceLog
            {
                CheckOut =
                    DateTime.UtcNow
            });

    Assert.ThrowsAsync<
        BusinessException>(
        async () =>
            await service
                .CheckOutAsync(
                    Guid.NewGuid()));
}

[Test]
public async Task GetAttendanceAsync_ShouldReturnPaginatedAttendance()
{
    var query =
        new AttendanceQueryDto
        {
            Page = 1,
            PageSize = 10
        };

    repository
        .Setup(x =>
            x.GetAttendanceAsync(
                query,
                0,
                10))
        .ReturnsAsync(
            new List<AttendanceLog>
            {
                new AttendanceLog
                {
                    Id = Guid.NewGuid(),

                    AttendanceDate =
                        DateOnly.FromDateTime(
                            DateTime.UtcNow),

                    Status = "Present",

                    Employee =
                        new Employee
                        {
                            FirstName = "John",
                            LastName = "Doe"
                        }
                }
            });

    repository
        .Setup(x =>
            x.GetAttendanceCountAsync(
                query))
        .ReturnsAsync(1);

    var result =
        await service
            .GetAttendanceAsync(
                query);

    Assert.That(
        result.TotalRecords,
        Is.EqualTo(1));

    Assert.That(
        result.Data.Count,
        Is.EqualTo(1));

    Assert.That(
        result.Data[0].EmployeeName,
        Is.EqualTo("John Doe"));
}

[Test]
public void GetAttendanceAsync_ShouldThrowValidationException_WhenPageInvalid()
{
    Assert.ThrowsAsync<
        ValidationException>(
        async () =>
            await service
                .GetAttendanceAsync(
                    new AttendanceQueryDto
                    {
                        Page = 0,
                        PageSize = 10
                    }));
}

[Test]
public void GetAttendanceAsync_ShouldThrowValidationException_WhenPageSizeInvalid()
{
    Assert.ThrowsAsync<
        ValidationException>(
        async () =>
            await service
                .GetAttendanceAsync(
                    new AttendanceQueryDto
                    {
                        Page = 1,
                        PageSize = 0
                    }));
}

[Test]
public void GetAttendanceAsync_ShouldThrowValidationException_WhenFromDateGreaterThanToDate()
{
    Assert.ThrowsAsync<
        ValidationException>(
        async () =>
            await service
                .GetAttendanceAsync(
                    new AttendanceQueryDto
                    {
                        Page = 1,
                        PageSize = 10,
                        FromDate =
                            DateOnly.FromDateTime(
                                DateTime.UtcNow.AddDays(5)),
                        ToDate =
                            DateOnly.FromDateTime(
                                DateTime.UtcNow)
                    }));
}

[Test]
public void GetAttendanceAsync_ShouldThrowValidationException_WhenStatusInvalid()
{
    Assert.ThrowsAsync<
        ValidationException>(
        async () =>
            await service
                .GetAttendanceAsync(
                    new AttendanceQueryDto
                    {
                        Page = 1,
                        PageSize = 10,
                        Status = "XYZ"
                    }));
}

[Test]
public async Task GetEmployeeAttendanceAsync_ShouldReturnAttendance()
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
            x.GetEmployeeAttendanceAsync(
                employeeId))
        .ReturnsAsync(
            new List<AttendanceLog>
            {
                new AttendanceLog
                {
                    Employee =
                        new Employee
                        {
                            FirstName = "John",
                            LastName = "Doe"
                        },

                    Status =
                        "Present"
                }
            });

    var result =
        await service
            .GetEmployeeAttendanceAsync(
                employeeId);

    Assert.That(
        result.Count,
        Is.EqualTo(1));
}

[Test]
public void GetEmployeeAttendanceAsync_ShouldThrowNotFoundException_WhenEmployeeNotFound()
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
                .GetEmployeeAttendanceAsync(
                    Guid.NewGuid()));
}


}

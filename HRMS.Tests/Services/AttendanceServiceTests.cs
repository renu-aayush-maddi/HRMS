using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using HRMS.API.Exceptions;
using HRMS.API.Models.DTOs.Attendance;

namespace HRMS.Tests.Services;

[TestFixture]
public class AttendanceServiceTests
{
    [Test]
    public void CheckIn_ShouldAddAttendance_WhenValid()
    {
        var repository =
            new Mock<IAttendanceRepository>();

        var employee =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns(employee);

        repository
            .Setup(x =>
                x.GetTodayAttendance(
                    employee.Id))
            .Returns((AttendanceLog?)null);

        var service =
            new AttendanceService(
                repository.Object);

        service.CheckIn(
            Guid.NewGuid());

        repository.Verify(
            x => x.AddAttendance(
                It.IsAny<AttendanceLog>()),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void CheckIn_ShouldThrowNotFoundException_WhenEmployeeNotFound()
    {
        var repository =
            new Mock<IAttendanceRepository>();

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns((Employee?)null);

        var service =
            new AttendanceService(
                repository.Object);

        Assert.Throws<NotFoundException>(
            () =>
            service.CheckIn(
                Guid.NewGuid()));
    }

    [Test]
    public void CheckIn_ShouldThrowBusinessException_WhenAlreadyCheckedIn()
    {
        var repository =
            new Mock<IAttendanceRepository>();

        var employee =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns(employee);

        repository
            .Setup(x =>
                x.GetTodayAttendance(
                    employee.Id))
            .Returns(
                new AttendanceLog());

        var service =
            new AttendanceService(
                repository.Object);

        Assert.Throws<BusinessException>(
            () =>
            service.CheckIn(
                Guid.NewGuid()));
    }

    [Test]
    public void CheckOut_ShouldUpdateAttendance_WhenValid()
    {
        var repository =
            new Mock<IAttendanceRepository>();

        var employee =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        var attendance =
            new AttendanceLog
            {
                CheckOut = null
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns(employee);

        repository
            .Setup(x =>
                x.GetTodayAttendance(
                    employee.Id))
            .Returns(attendance);

        var service =
            new AttendanceService(
                repository.Object);

        service.CheckOut(
            Guid.NewGuid());

        repository.Verify(
            x => x.UpdateAttendance(
                attendance),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void CheckOut_ShouldThrowNotFoundException_WhenEmployeeNotFound()
    {
        var repository =
            new Mock<IAttendanceRepository>();

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns((Employee?)null);

        var service =
            new AttendanceService(
                repository.Object);

        Assert.Throws<NotFoundException>(
            () =>
            service.CheckOut(
                Guid.NewGuid()));
    }

    [Test]
    public void CheckOut_ShouldThrowNotFoundException_WhenAttendanceNotFound()
    {
        var repository =
            new Mock<IAttendanceRepository>();

        var employee =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns(employee);

        repository
            .Setup(x =>
                x.GetTodayAttendance(
                    employee.Id))
            .Returns((AttendanceLog?)null);

        var service =
            new AttendanceService(
                repository.Object);

        Assert.Throws<NotFoundException>(
            () =>
            service.CheckOut(
                Guid.NewGuid()));
    }

    [Test]
    public void CheckOut_ShouldThrowBusinessException_WhenAlreadyCheckedOut()
    {
        var repository =
            new Mock<IAttendanceRepository>();

        var employee =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns(employee);

        repository
            .Setup(x =>
                x.GetTodayAttendance(
                    employee.Id))
            .Returns(
                new AttendanceLog
                {
                    CheckOut =
                        DateTime.Now
                });

        var service =
            new AttendanceService(
                repository.Object);

        Assert.Throws<BusinessException>(
            () =>
            service.CheckOut(
                Guid.NewGuid()));
    }

    [Test]
    public void GetAttendance_ShouldReturnEmptyList_WhenNoRecordsExist()
    {
        var repository =
            new Mock<IAttendanceRepository>();

        repository
            .Setup(x =>
                x.GetAttendance(
                    It.IsAny<AttendanceQueryDto>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
            .Returns(new List<AttendanceLog>());

        repository
            .Setup(x =>
                x.GetAttendanceCount(
                    It.IsAny<AttendanceQueryDto>()))
            .Returns(0);

        var service =
            new AttendanceService(
                repository.Object);

        var result =
            service.GetAttendance(
                new AttendanceQueryDto());

        Assert.That(
            result.Data.Count,
            Is.EqualTo(0));

        Assert.That(
            result.TotalRecords,
            Is.EqualTo(0));
    }

    [Test]
    public void GetAttendance_ShouldMapEntityToDto()
    {
        var repository =
            new Mock<IAttendanceRepository>();

        var employeeId =
            Guid.NewGuid();

        repository
            .Setup(x =>
                x.GetAttendance(
                    It.IsAny<AttendanceQueryDto>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
            .Returns(
                new List<AttendanceLog>
                {
                    new AttendanceLog
                    {
                        Id = Guid.NewGuid(),

                        EmployeeId =
                            employeeId,

                        AttendanceDate =
                            new DateOnly(
                                2026,
                                1,
                                1),

                        Status =
                            "Present",

                        Employee =
                            new Employee
                            {
                                FirstName =
                                    "John",

                                LastName =
                                    "Doe"
                            }
                    }
                });

        repository
            .Setup(x =>
                x.GetAttendanceCount(
                    It.IsAny<AttendanceQueryDto>()))
            .Returns(1);

        var service =
            new AttendanceService(
                repository.Object);

        var result =
            service.GetAttendance(
                new AttendanceQueryDto());

        Assert.That(
            result.Data.Count,
            Is.EqualTo(1));

        Assert.That(
            result.Data[0].EmployeeName,
            Is.EqualTo("John Doe"));

        Assert.That(
            result.Data[0].Status,
            Is.EqualTo("Present"));
    }

    [Test]
    public void GetAttendance_ShouldCalculateSkipCorrectly()
    {
        var repository =
            new Mock<IAttendanceRepository>();

        repository
            .Setup(x =>
                x.GetAttendance(
                    It.IsAny<AttendanceQueryDto>(),
                    10,
                    10))
            .Returns(
                new List<AttendanceLog>());

        repository
            .Setup(x =>
                x.GetAttendanceCount(
                    It.IsAny<AttendanceQueryDto>()))
            .Returns(0);

        var service =
            new AttendanceService(
                repository.Object);

        service.GetAttendance(
            new AttendanceQueryDto
            {
                Page = 2,
                PageSize = 10
            });

        repository.Verify(
            x =>
            x.GetAttendance(
                It.IsAny<AttendanceQueryDto>(),
                10,
                10),
            Times.Once);
    }

    [Test]
    public void GetAttendance_ShouldUsePageOne_WhenPageIsInvalid()
    {
        var repository =
            new Mock<IAttendanceRepository>();

        repository
            .Setup(x =>
                x.GetAttendance(
                    It.IsAny<AttendanceQueryDto>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
            .Returns(
                new List<AttendanceLog>());

        repository
            .Setup(x =>
                x.GetAttendanceCount(
                    It.IsAny<AttendanceQueryDto>()))
            .Returns(0);

        var service =
            new AttendanceService(
                repository.Object);

        var result =
            service.GetAttendance(
                new AttendanceQueryDto
                {
                    Page = -5
                });

        Assert.That(
            result.Page,
            Is.EqualTo(1));
    }
}
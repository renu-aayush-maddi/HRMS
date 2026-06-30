using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.Manager;

namespace HRMS.Tests.Services;

[TestFixture]
public partial class ManagerServiceTests
{
    private Mock<IManagerRepository>
        repository;

    private ManagerService
        service;

    [SetUp]
    public void Setup()
    {
        repository =
            new Mock<IManagerRepository>();

        service =
            new ManagerService(
                repository.Object);
    }

    [Test]
    public void GetDashboard_ShouldReturnDashboard_WhenManagerExists()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        var dashboard =
            new ManagerDashboardDto
            {
                TeamSize = 5,
                PresentToday = 4
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetDashboard(
                    manager.Id))
            .Returns(dashboard);

        var result =
            service.GetDashboard(
                manager.Id);

        Assert.That(
            result.TeamSize,
            Is.EqualTo(5));

        Assert.That(
            result.PresentToday,
            Is.EqualTo(4));
    }

    [Test]
    public void GetDashboard_ShouldThrowException_WhenManagerNotFound()
    {
        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns(
                (Employee?)null);

        Assert.Throws<Exception>(
            () =>
                service.GetDashboard(
                    Guid.NewGuid()));
    }

    [Test]
    public void GetTeamMembers_ShouldReturnMappedDtos()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetTeamMembers(
                    manager.Id))
            .Returns(
            [
                new Employee
                {
                    Id = Guid.NewGuid(),

                    EmployeeCode =
                        "EMP001",

                    FirstName =
                        "John",

                    LastName =
                        "Doe",

                    Email =
                        "john@test.com",

                    Designation =
                        "Developer",

                    EmploymentStatus =
                        "Active",

                    Department =
                        new Department
                        {
                            Name = "IT"
                        }
                }
            ]);

        var result =
            service.GetTeamMembers(
                manager.Id);

        Assert.That(
            result.Count,
            Is.EqualTo(1));

        Assert.That(
            result[0].FullName,
            Is.EqualTo("John Doe"));

        Assert.That(
            result[0].Department,
            Is.EqualTo("IT"));
    }

    [Test]
    public void GetTeamMembers_ShouldReturnEmptyList_WhenNoTeamMembersExist()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetTeamMembers(
                    manager.Id))
            .Returns([]);

        var result =
            service.GetTeamMembers(
                manager.Id);

        Assert.That(
            result,
            Is.Empty);
    }

    [Test]
    public void GetTeamMembers_ShouldThrowException_WhenManagerNotFound()
    {
        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns(
                (Employee?)null);

        Assert.Throws<Exception>(
            () =>
                service.GetTeamMembers(
                    Guid.NewGuid()));
    }

    [Test]
    public void GetTeamMember_ShouldReturnMappedDto()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        var employee =
            new Employee
            {
                Id = Guid.NewGuid(),

                EmployeeCode =
                    "EMP001",

                FirstName =
                    "John",

                LastName =
                    "Doe",

                Email =
                    "john@test.com",

                Designation =
                    "Developer",

                EmploymentStatus =
                    "Active",

                Department =
                    new Department
                    {
                        Name = "IT"
                    }
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetTeamMember(
                    manager.Id,
                    employee.Id))
            .Returns(employee);

        var result =
            service.GetTeamMember(
                manager.Id,
                employee.Id);

        Assert.That(
            result.FullName,
            Is.EqualTo("John Doe"));

        Assert.That(
            result.Department,
            Is.EqualTo("IT"));
    }

    [Test]
    public void GetTeamMember_ShouldThrowException_WhenManagerNotFound()
    {
        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns(
                (Employee?)null);

        Assert.Throws<Exception>(
            () =>
                service.GetTeamMember(
                    Guid.NewGuid(),
                    Guid.NewGuid()));
    }

    [Test]
    public void GetTeamMember_ShouldThrowException_WhenEmployeeNotFound()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetTeamMember(
                    manager.Id,
                    It.IsAny<Guid>()))
            .Returns(
                (Employee?)null);

        Assert.Throws<Exception>(
            () =>
                service.GetTeamMember(
                    manager.Id,
                    Guid.NewGuid()));
    }

    [Test]
    public void GetTeamAttendance_ShouldReturnMappedDtos()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetTeamAttendance(
                    manager.Id))
            .Returns(
            [
                new AttendanceLog
            {
                Id = Guid.NewGuid(),

                AttendanceDate =
                    new DateOnly(2025, 1, 1),

                CheckIn =
                    DateTime.UtcNow,

                Status =
                    "Present",

                Employee =
                    new Employee
                    {
                        FirstName = "John",
                        LastName = "Doe"
                    }
            }
            ]);

        var result =
            service.GetTeamAttendance(
                manager.Id);

        Assert.That(
            result.Count,
            Is.EqualTo(1));

        Assert.That(
            result[0].EmployeeName,
            Is.EqualTo("John Doe"));
    }

    [Test]
    public void GetTeamAttendance_ShouldReturnEmptyList_WhenNoRecordsExist()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetTeamAttendance(
                    manager.Id))
            .Returns([]);

        var result =
            service.GetTeamAttendance(
                manager.Id);

        Assert.That(
            result,
            Is.Empty);
    }

    [Test]
    public void GetTeamAttendance_ShouldThrowException_WhenManagerNotFound()
    {
        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns(
                (Employee?)null);

        Assert.Throws<Exception>(
            () =>
                service.GetTeamAttendance(
                    Guid.NewGuid()));
    }

    [Test]
    public void GetLateEmployees_ShouldReturnMappedDtos()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetLateEmployees(
                    manager.Id))
            .Returns(
            [
                new AttendanceLog
            {
                CheckIn =
                    DateTime.UtcNow,

                Employee =
                    new Employee
                    {
                        FirstName = "Jane",
                        LastName = "Smith"
                    }
            }
            ]);

        var result =
            service.GetLateEmployees(
                manager.Id);

        Assert.That(
            result.Count,
            Is.EqualTo(1));

        Assert.That(
            result[0].EmployeeName,
            Is.EqualTo("Jane Smith"));
    }

    [Test]
    public void GetLateEmployees_ShouldReturnEmptyList_WhenNoLateEmployeesExist()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetLateEmployees(
                    manager.Id))
            .Returns([]);

        var result =
            service.GetLateEmployees(
                manager.Id);

        Assert.That(
            result,
            Is.Empty);
    }

    [Test]
    public void GetLateEmployees_ShouldThrowException_WhenManagerNotFound()
    {
        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns(
                (Employee?)null);

        Assert.Throws<Exception>(
            () =>
                service.GetLateEmployees(
                    Guid.NewGuid()));
    }
    [Test]
    public void GetPendingLeaveRequests_ShouldReturnMappedDtos()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetPendingLeaveRequests(
                    manager.Id))
            .Returns(
            [
                new LeaveRequest
            {
                Id = Guid.NewGuid(),

                FromDate =
                    new DateOnly(2025, 1, 10),

                ToDate =
                    new DateOnly(2025, 1, 12),

                Reason =
                    "Medical",

                Status =
                    "Pending",

                Employee =
                    new Employee
                    {
                        FirstName = "John",
                        LastName = "Doe"
                    }
            }
            ]);

        var result =
            service.GetPendingLeaveRequests(
                manager.Id);

        Assert.That(
            result.Count,
            Is.EqualTo(1));

        Assert.That(
            result[0].EmployeeName,
            Is.EqualTo("John Doe"));
    }

    [Test]
    public void GetPendingLeaveRequests_ShouldReturnEmptyList_WhenNoRequestsExist()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetPendingLeaveRequests(
                    manager.Id))
            .Returns([]);

        var result =
            service.GetPendingLeaveRequests(
                manager.Id);

        Assert.That(
            result,
            Is.Empty);
    }

    [Test]
    public void GetPendingLeaveRequests_ShouldThrowException_WhenManagerNotFound()
    {
        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns(
                (Employee?)null);

        Assert.Throws<Exception>(
            () =>
                service.GetPendingLeaveRequests(
                    Guid.NewGuid()));
    }
    [Test]
    public void AddPerformanceReview_ShouldAddReview_WhenValid()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        var employee =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        var dto =
            new AddPerformanceReviewDto
            {
                EmployeeId =
                    employee.Id,

                Rating = 4,

                Comments =
                    "Excellent"
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetTeamMember(
                    manager.Id,
                    employee.Id))
            .Returns(employee);

        service.AddPerformanceReview(
            manager.Id,
            dto);

        repository.Verify(
            x => x.AddPerformanceReview(
                It.IsAny<PerformanceReview>()),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void AddPerformanceReview_ShouldThrowException_WhenManagerNotFound()
    {
        var dto =
            new AddPerformanceReviewDto
            {
                EmployeeId =
                    Guid.NewGuid(),

                Rating = 4
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns(
                (Employee?)null);

        Assert.Throws<Exception>(
            () =>
                service.AddPerformanceReview(
                    Guid.NewGuid(),
                    dto));
    }

    [Test]
    public void AddPerformanceReview_ShouldThrowException_WhenEmployeeNotInTeam()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        var dto =
            new AddPerformanceReviewDto
            {
                EmployeeId =
                    Guid.NewGuid(),

                Rating = 4
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetTeamMember(
                    manager.Id,
                    dto.EmployeeId))
            .Returns(
                (Employee?)null);

        Assert.Throws<Exception>(
            () =>
                service.AddPerformanceReview(
                    manager.Id,
                    dto));
    }

    [Test]
    public void AddPerformanceReview_ShouldThrowException_WhenRatingLessThanOne()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        var employee =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        var dto =
            new AddPerformanceReviewDto
            {
                EmployeeId =
                    employee.Id,

                Rating = 0
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetTeamMember(
                    manager.Id,
                    employee.Id))
            .Returns(employee);

        Assert.Throws<Exception>(
            () =>
                service.AddPerformanceReview(
                    manager.Id,
                    dto));
    }

    [Test]
    public void AddPerformanceReview_ShouldThrowException_WhenRatingGreaterThanFive()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        var employee =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        var dto =
            new AddPerformanceReviewDto
            {
                EmployeeId =
                    employee.Id,

                Rating = 6
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetTeamMember(
                    manager.Id,
                    employee.Id))
            .Returns(employee);

        Assert.Throws<Exception>(
            () =>
                service.AddPerformanceReview(
                    manager.Id,
                    dto));
    }


    [Test]
    public void GetPerformanceReviews_ShouldReturnMappedDtos()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetPerformanceReviews(
                    manager.Id))
            .Returns(
            [
                new PerformanceReview
            {
                Id = Guid.NewGuid(),

                Rating = 4,

                Comments =
                    "Good Work",

                ReviewDate =
                    new DateOnly(2025, 1, 1),

                Employee =
                    new Employee
                    {
                        FirstName = "John",
                        LastName = "Doe"
                    }
            }
            ]);

        var result =
            service.GetPerformanceReviews(
                manager.Id);

        Assert.That(
            result.Count,
            Is.EqualTo(1));

        Assert.That(
            result[0].EmployeeName,
            Is.EqualTo("John Doe"));

        Assert.That(
            result[0].Rating,
            Is.EqualTo(4));
    }

    [Test]
    public void GetPerformanceReviews_ShouldReturnEmptyList()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetPerformanceReviews(
                    manager.Id))
            .Returns([]);

        var result =
            service.GetPerformanceReviews(
                manager.Id);

        Assert.That(
            result,
            Is.Empty);
    }

    [Test]
    public void GetPerformanceReviews_ShouldThrowException_WhenManagerNotFound()
    {
        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns(
                (Employee?)null);

        Assert.Throws<Exception>(
            () =>
                service.GetPerformanceReviews(
                    Guid.NewGuid()));
    }

    [Test]
    public void GetEmployeePerformanceReviews_ShouldReturnMappedDtos()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        var employeeId =
            Guid.NewGuid();

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetEmployeePerformanceReviews(
                    manager.Id,
                    employeeId))
            .Returns(
            [
                new PerformanceReview
            {
                Id = Guid.NewGuid(),

                Rating = 5,

                Comments =
                    "Excellent",

                ReviewDate =
                    new DateOnly(2025, 1, 1),

                Employee =
                    new Employee
                    {
                        FirstName = "Jane",
                        LastName = "Smith"
                    }
            }
            ]);

        var result =
            service.GetEmployeePerformanceReviews(
                manager.Id,
                employeeId);

        Assert.That(
            result.Count,
            Is.EqualTo(1));

        Assert.That(
            result[0].EmployeeName,
            Is.EqualTo("Jane Smith"));

        Assert.That(
            result[0].Rating,
            Is.EqualTo(5));
    }

    [Test]
    public void GetEmployeePerformanceReviews_ShouldReturnEmptyList()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetEmployeePerformanceReviews(
                    manager.Id,
                    It.IsAny<Guid>()))
            .Returns([]);

        var result =
            service.GetEmployeePerformanceReviews(
                manager.Id,
                Guid.NewGuid());

        Assert.That(
            result,
            Is.Empty);
    }

    [Test]
    public void GetEmployeePerformanceReviews_ShouldThrowException_WhenManagerNotFound()
    {
        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns(
                (Employee?)null);

        Assert.Throws<Exception>(
            () =>
                service.GetEmployeePerformanceReviews(
                    Guid.NewGuid(),
                    Guid.NewGuid()));
    }
}
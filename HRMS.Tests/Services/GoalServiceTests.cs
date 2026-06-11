using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.Goal;
using HRMS.API.Exceptions;
using HRMS.API.Models.DTOs.Common;


namespace HRMS.Tests.Services;

[TestFixture]
public class GoalServiceTests
{
    [Test]
public void AddGoal_ShouldAddGoal_WhenValid()
{
    var repository =
        new Mock<IGoalRepository>();

    var manager =
        new Employee
        {
            Id = Guid.NewGuid()
        };

    var employee =
        new Employee
        {
            Id = Guid.NewGuid(),
            ManagerId = manager.Id
        };

    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns(manager);

    repository
        .Setup(x =>
            x.GetTeamMember(
                manager.Id,
                employee.Id))
        .Returns(employee);

    var service =
        new GoalService(
            repository.Object);

    service.AddGoal(
        Guid.NewGuid(),
        new AddGoalDto
        {
            EmployeeId =
                employee.Id,

            Title =
                "Learn ASP.NET",

            TargetDate =
                DateOnly.FromDateTime(
                    DateTime.Today.AddDays(10))
        });

    repository.Verify(
        x => x.AddGoal(
            It.IsAny<EmployeeGoal>()),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}
[Test]
public void AddGoal_ShouldThrowNotFoundException_WhenManagerNotFound()
{
    var repository =
        new Mock<IGoalRepository>();

    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns((Employee?)null);

    var service =
        new GoalService(
            repository.Object);

    Assert.Throws<
        NotFoundException>(
        () =>
        service.AddGoal(
            Guid.NewGuid(),
            new AddGoalDto()));
}
[Test]
public void AddGoal_ShouldThrowBusinessException_WhenEmployeeNotInTeam()
{
    var repository =
        new Mock<IGoalRepository>();

    var manager =
        new Employee
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns(manager);

    repository
        .Setup(x =>
            x.GetTeamMember(
                It.IsAny<Guid>(),
                It.IsAny<Guid>()))
        .Returns((Employee?)null);

    var service =
        new GoalService(
            repository.Object);

    Assert.Throws<
        BusinessException>(
        () =>
        service.AddGoal(
            Guid.NewGuid(),
            new AddGoalDto()));
}
[Test]
public void AddGoal_ShouldThrowBusinessException_WhenTargetDatePast()
{
    var repository =
        new Mock<IGoalRepository>();

    var manager =
        new Employee
        {
            Id = Guid.NewGuid()
        };

    var employee =
        new Employee
        {
            Id = Guid.NewGuid(),
            ManagerId = manager.Id
        };

    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns(manager);

    repository
        .Setup(x =>
            x.GetTeamMember(
                manager.Id,
                employee.Id))
        .Returns(employee);

    var service =
        new GoalService(
            repository.Object);

    Assert.Throws<
        BusinessException>(
        () =>
        service.AddGoal(
            Guid.NewGuid(),
            new AddGoalDto
            {
                EmployeeId =
                    employee.Id,

                Title =
                    "Test",

                TargetDate =
                    DateOnly.FromDateTime(
                        DateTime.Today.AddDays(-1))
            }));
}
[Test]
public void GetGoals_ShouldReturnMappedDtos()
{
    var repository =
        new Mock<IGoalRepository>();

    var manager =
        new Employee
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns(manager);

    repository
        .Setup(x =>
            x.GetGoals(
                It.IsAny<Guid>(),
                It.IsAny<GoalQueryDto>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
        .Returns(
            new List<EmployeeGoal>
            {
                new EmployeeGoal
                {
                    Id = Guid.NewGuid(),

                    Title = "Goal",

                    Status = "Assigned",

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
            x.GetGoalsCount(
                It.IsAny<Guid>(),
                It.IsAny<GoalQueryDto>()))
        .Returns(1);

    var service =
        new GoalService(
            repository.Object);

    var result =
        service.GetGoals(
            Guid.NewGuid(),
            new GoalQueryDto());

    Assert.That(
        result.Data.Count,
        Is.EqualTo(1));

    Assert.That(
        result.Data[0]
            .EmployeeName,
        Is.EqualTo(
            "John Doe"));
}

[Test]
public void GetGoals_ShouldThrowNotFoundException_WhenManagerNotFound()
{
    var repository =
        new Mock<IGoalRepository>();

    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns((Employee?)null);

    var service =
        new GoalService(
            repository.Object);

    Assert.Throws<NotFoundException>(
        () =>
        service.GetGoals(
            Guid.NewGuid(),
            new GoalQueryDto()));
}

[Test]
public void GetEmployeeGoals_ShouldReturnMappedDtos()
{
    var repository =
        new Mock<IGoalRepository>();

    var manager =
        new Employee
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns(manager);

    repository
        .Setup(x =>
            x.GetEmployeeGoals(
                manager.Id,
                It.IsAny<Guid>()))
        .Returns(
            new List<EmployeeGoal>
            {
                new EmployeeGoal
                {
                    Id = Guid.NewGuid(),

                    Title = "ASP.NET",

                    Description = "Learn APIs",

                    Status = "Assigned",

                    Employee =
                        new Employee
                        {
                            FirstName = "John",
                            LastName = "Doe"
                        }
                }
            });

    var service =
        new GoalService(
            repository.Object);

    var result =
        service.GetEmployeeGoals(
            Guid.NewGuid(),
            Guid.NewGuid());

    Assert.That(
        result.Count,
        Is.EqualTo(1));

    Assert.That(
        result[0].EmployeeName,
        Is.EqualTo("John Doe"));

    Assert.That(
        result[0].Title,
        Is.EqualTo("ASP.NET"));
}
[Test]
public void GetEmployeeGoals_ShouldThrowNotFoundException_WhenManagerNotFound()
{
    var repository =
        new Mock<IGoalRepository>();

    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns((Employee?)null);

    var service =
        new GoalService(
            repository.Object);

    Assert.Throws<NotFoundException>(
        () =>
        service.GetEmployeeGoals(
            Guid.NewGuid(),
            Guid.NewGuid()));
}
[Test]
public void UpdateGoalStatus_ShouldUpdateGoal_WhenValid()
{
    var repository =
        new Mock<IGoalRepository>();

    var manager =
        new Employee
        {
            Id = Guid.NewGuid()
        };

    var goal =
        new EmployeeGoal
        {
            Id = Guid.NewGuid(),

            Status = "Assigned",

            Employee =
                new Employee
                {
                    ManagerId =
                        manager.Id
                }
        };

    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns(manager);

    repository
        .Setup(x =>
            x.GetGoal(goal.Id))
        .Returns(goal);

    var service =
        new GoalService(
            repository.Object);

    service.UpdateGoalStatus(
        Guid.NewGuid(),
        goal.Id,
        new UpdateGoalStatusDto
        {
            Status = "Completed"
        });

    Assert.That(
        goal.Status,
        Is.EqualTo("Completed"));

    repository.Verify(
        x => x.UpdateGoal(goal),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}
[Test]
public void UpdateGoalStatus_ShouldThrowNotFoundException_WhenManagerNotFound()
{
    var repository =
        new Mock<IGoalRepository>();

    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns((Employee?)null);

    var service =
        new GoalService(
            repository.Object);

    Assert.Throws<NotFoundException>(
        () =>
        service.UpdateGoalStatus(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new UpdateGoalStatusDto
            {
                Status = "Completed"
            }));
}
[Test]
public void UpdateGoalStatus_ShouldThrowNotFoundException_WhenGoalNotFound()
{
    var repository =
        new Mock<IGoalRepository>();

    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns(
            new Employee
            {
                Id = Guid.NewGuid()
            });

    repository
        .Setup(x =>
            x.GetGoal(
                It.IsAny<Guid>()))
        .Returns((EmployeeGoal?)null);

    var service =
        new GoalService(
            repository.Object);

    Assert.Throws<NotFoundException>(
        () =>
        service.UpdateGoalStatus(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new UpdateGoalStatusDto
            {
                Status = "Completed"
            }));
}
[Test]
public void UpdateGoalStatus_ShouldThrowBusinessException_WhenUnauthorized()
{
    var repository =
        new Mock<IGoalRepository>();

    var manager =
        new Employee
        {
            Id = Guid.NewGuid()
        };

    var goal =
        new EmployeeGoal
        {
            Employee =
                new Employee
                {
                    ManagerId =
                        Guid.NewGuid()
                }
        };

    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns(manager);

    repository
        .Setup(x =>
            x.GetGoal(
                It.IsAny<Guid>()))
        .Returns(goal);

    var service =
        new GoalService(
            repository.Object);

    Assert.Throws<UnauthorizedAccessException>(
        () =>
        service.UpdateGoalStatus(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new UpdateGoalStatusDto
            {
                Status = "Completed"
            }));
}
[Test]
public void UpdateGoalStatus_ShouldThrowBusinessException_WhenStatusInvalid()
{
    var repository =
        new Mock<IGoalRepository>();

    var manager =
        new Employee
        {
            Id = Guid.NewGuid()
        };

    var goal =
        new EmployeeGoal
        {
            Employee =
                new Employee
                {
                    ManagerId =
                        manager.Id
                }
        };

    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns(manager);

    repository
        .Setup(x =>
            x.GetGoal(
                It.IsAny<Guid>()))
        .Returns(goal);

    var service =
        new GoalService(
            repository.Object);

    Assert.Throws<BusinessException>(
        () =>
        service.UpdateGoalStatus(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new UpdateGoalStatusDto
            {
                Status = "RandomStatus"
            }));
}
}
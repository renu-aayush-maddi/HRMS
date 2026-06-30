using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Exceptions;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.Bonus;

namespace HRMS.Tests.Services;

[TestFixture]
public class BonusServiceTests
{
private Mock<IBonusRepository>
repository;


private Mock<INotificationService>
    notificationService;

private BonusService
    service;

[SetUp]
public void Setup()
{
    repository =
        new Mock<IBonusRepository>();

    notificationService =
        new Mock<INotificationService>();

    service =
        new BonusService(
            repository.Object,
            notificationService.Object);
}

[Test]
public void CreateBonus_ShouldCreateBonus_WhenValid()
{
    var employee =
        new Employee
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetEmployee(
                employee.Id))
        .Returns(employee);

    var dto =
        new CreateBonusDto
        {
            EmployeeId =
                employee.Id,

            Amount = 5000,

            Reason = "Performance",

            BonusMonth = 6,

            BonusYear = 2025
        };

    service.CreateBonus(dto);

    repository.Verify(
        x => x.AddBonus(
            It.IsAny<Bonuse>()),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void CreateBonus_ShouldThrowNotFoundException_WhenEmployeeNotFound()
{
    repository
        .Setup(x =>
            x.GetEmployee(
                It.IsAny<Guid>()))
        .Returns(
            (Employee?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.CreateBonus(
                new CreateBonusDto
                {
                    EmployeeId =
                        Guid.NewGuid()
                }));
}

[Test]
public void ApproveBonus_ShouldApproveBonus_WhenValid()
{
    var userId =
        Guid.NewGuid();

    var bonus =
        new Bonuse
        {
            Id = Guid.NewGuid(),

            Amount = 5000,

            Status = "Pending",

            Employee =
                new Employee
                {
                    UserId = userId
                }
        };

    repository
        .Setup(x =>
            x.GetBonus(
                bonus.Id))
        .Returns(bonus);

    service.ApproveBonus(
        bonus.Id);

    Assert.That(
        bonus.Status,
        Is.EqualTo(
            "Approved"));

    repository.Verify(
        x => x.UpdateBonus(
            bonus),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void ApproveBonus_ShouldThrowNotFoundException_WhenBonusNotFound()
{
    repository
        .Setup(x =>
            x.GetBonus(
                It.IsAny<Guid>()))
        .Returns(
            (Bonuse?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.ApproveBonus(
                Guid.NewGuid()));
}

[Test]
public void ApproveBonus_ShouldThrowBusinessException_WhenBonusAlreadyProcessed()
{
    repository
        .Setup(x =>
            x.GetBonus(
                It.IsAny<Guid>()))
        .Returns(
            new Bonuse
            {
                Status = "Approved"
            });

    Assert.Throws<
        BusinessException>(
        () =>
            service.ApproveBonus(
                Guid.NewGuid()));
}

[Test]
public void ApproveBonus_ShouldCreateNotification_WhenEmployeeHasUserId()
{
    var userId =
        Guid.NewGuid();

    var bonus =
        new Bonuse
        {
            Id = Guid.NewGuid(),

            Amount = 5000,

            Status = "Pending",

            Employee =
                new Employee
                {
                    UserId = userId
                }
        };

    repository
        .Setup(x =>
            x.GetBonus(
                bonus.Id))
        .Returns(bonus);

    service.ApproveBonus(
        bonus.Id);

    notificationService.Verify(
        x =>
            x.CreateNotification(
                userId,
                "Bonus Approved",
                It.IsAny<string>()),
        Times.Once);
}

[Test]
public void RejectBonus_ShouldRejectBonus_WhenValid()
{
    var bonus =
        new Bonuse
        {
            Id = Guid.NewGuid(),

            Status = "Pending"
        };

    repository
        .Setup(x =>
            x.GetBonus(
                bonus.Id))
        .Returns(bonus);

    service.RejectBonus(
        bonus.Id);

    Assert.That(
        bonus.Status,
        Is.EqualTo(
            "Rejected"));

    repository.Verify(
        x => x.UpdateBonus(
            bonus),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void RejectBonus_ShouldThrowNotFoundException_WhenBonusNotFound()
{
    repository
        .Setup(x =>
            x.GetBonus(
                It.IsAny<Guid>()))
        .Returns(
            (Bonuse?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.RejectBonus(
                Guid.NewGuid()));
}

[Test]
public void RejectBonus_ShouldThrowBusinessException_WhenBonusAlreadyProcessed()
{
    repository
        .Setup(x =>
            x.GetBonus(
                It.IsAny<Guid>()))
        .Returns(
            new Bonuse
            {
                Status = "Approved"
            });

    Assert.Throws<
        BusinessException>(
        () =>
            service.RejectBonus(
                Guid.NewGuid()));
}

[Test]
public void GetAllBonuses_ShouldReturnMappedDtos()
{
    repository
        .Setup(x =>
            x.GetAllBonuses())
        .Returns(
            new List<Bonuse>
            {
                new Bonuse
                {
                    Id = Guid.NewGuid(),

                    Amount = 5000,

                    Reason = "Performance",

                    BonusMonth = 6,

                    BonusYear = 2025,

                    Status = "Approved",

                    Employee =
                        new Employee
                        {
                            FirstName = "John",
                            LastName = "Doe"
                        }
                }
            });

    var result =
        service.GetAllBonuses();

    Assert.That(
        result.Count,
        Is.EqualTo(1));

    Assert.That(
        result[0].EmployeeName,
        Is.EqualTo("John Doe"));

    Assert.That(
        result[0].Amount,
        Is.EqualTo(5000));
}

[Test]
public void GetMyBonuses_ShouldReturnMappedDtos()
{
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
            x.GetEmployeeBonuses(
                employee.Id))
        .Returns(
            new List<Bonuse>
            {
                new Bonuse
                {
                    Amount = 5000,

                    Status = "Approved",

                    Employee =
                        new Employee
                        {
                            FirstName = "John",
                            LastName = "Doe"
                        }
                }
            });

    var result =
        service.GetMyBonuses(
            Guid.NewGuid());

    Assert.That(
        result.Count,
        Is.EqualTo(1));
}

[Test]
public void GetMyBonuses_ShouldThrowNotFoundException_WhenEmployeeNotFound()
{
    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns(
            (Employee?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.GetMyBonuses(
                Guid.NewGuid()));
}


}

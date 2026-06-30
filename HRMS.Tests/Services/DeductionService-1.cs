using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Exceptions;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.Deduction;

namespace HRMS.Tests.Services;

[TestFixture]
public class DeductionServiceTests
{
private Mock<IDeductionRepository>
repository;


private Mock<INotificationService>
    notificationService;

private DeductionService
    service;

[SetUp]
public void Setup()
{
    repository =
        new Mock<IDeductionRepository>();

    notificationService =
        new Mock<INotificationService>();

    service =
        new DeductionService(
            repository.Object,
            notificationService.Object);
}

[Test]
public void CreateDeduction_ShouldCreateDeduction_WhenValid()
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
        new CreateDeductionDto
        {
            EmployeeId =
                employee.Id,

            Amount = 5000,

            Reason = "Late Coming",

            DeductionMonth = 6,

            DeductionYear = 2025
        };

    service.CreateDeduction(dto);

    repository.Verify(
        x => x.AddDeduction(
            It.IsAny<Deduction>()),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void CreateDeduction_ShouldThrowNotFoundException_WhenEmployeeNotFound()
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
            service.CreateDeduction(
                new CreateDeductionDto
                {
                    EmployeeId =
                        Guid.NewGuid()
                }));
}

[Test]
public void ApproveDeduction_ShouldApproveDeduction_WhenValid()
{
    var deduction =
        new Deduction
        {
            Id = Guid.NewGuid(),

            Status = "Pending",

            Amount = 5000,

            Employee =
                new Employee()
        };

    repository
        .Setup(x =>
            x.GetDeduction(
                deduction.Id))
        .Returns(deduction);

    service.ApproveDeduction(
        deduction.Id);

    Assert.That(
        deduction.Status,
        Is.EqualTo(
            "Approved"));

    repository.Verify(
        x => x.UpdateDeduction(
            deduction),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void ApproveDeduction_ShouldThrowNotFoundException_WhenDeductionNotFound()
{
    repository
        .Setup(x =>
            x.GetDeduction(
                It.IsAny<Guid>()))
        .Returns(
            (Deduction?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.ApproveDeduction(
                Guid.NewGuid()));
}

[Test]
public void ApproveDeduction_ShouldThrowBusinessException_WhenDeductionAlreadyProcessed()
{
    repository
        .Setup(x =>
            x.GetDeduction(
                It.IsAny<Guid>()))
        .Returns(
            new Deduction
            {
                Status = "Approved"
            });

    Assert.Throws<
        BusinessException>(
        () =>
            service.ApproveDeduction(
                Guid.NewGuid()));
}

[Test]
public void ApproveDeduction_ShouldCreateNotification_WhenEmployeeHasUserId()
{
    var userId =
        Guid.NewGuid();

    var deduction =
        new Deduction
        {
            Id = Guid.NewGuid(),

            Status = "Pending",

            Amount = 5000,

            Employee =
                new Employee
                {
                    UserId = userId
                }
        };

    repository
        .Setup(x =>
            x.GetDeduction(
                deduction.Id))
        .Returns(deduction);

    service.ApproveDeduction(
        deduction.Id);

    notificationService.Verify(
        x =>
            x.CreateNotification(
                userId,
                "Deduction Approved",
                It.IsAny<string>()),
        Times.Once);
}

[Test]
public void RejectDeduction_ShouldRejectDeduction_WhenValid()
{
    var deduction =
        new Deduction
        {
            Id = Guid.NewGuid(),

            Status = "Pending",

            Amount = 5000,

            Employee =
                new Employee()
        };

    repository
        .Setup(x =>
            x.GetDeduction(
                deduction.Id))
        .Returns(deduction);

    service.RejectDeduction(
        deduction.Id);

    Assert.That(
        deduction.Status,
        Is.EqualTo(
            "Rejected"));

    repository.Verify(
        x => x.UpdateDeduction(
            deduction),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void RejectDeduction_ShouldThrowNotFoundException_WhenDeductionNotFound()
{
    repository
        .Setup(x =>
            x.GetDeduction(
                It.IsAny<Guid>()))
        .Returns(
            (Deduction?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.RejectDeduction(
                Guid.NewGuid()));
}

[Test]
public void RejectDeduction_ShouldThrowBusinessException_WhenDeductionAlreadyProcessed()
{
    repository
        .Setup(x =>
            x.GetDeduction(
                It.IsAny<Guid>()))
        .Returns(
            new Deduction
            {
                Status = "Approved"
            });

    Assert.Throws<
        BusinessException>(
        () =>
            service.RejectDeduction(
                Guid.NewGuid()));
}

[Test]
public void RejectDeduction_ShouldCreateNotification_WhenEmployeeHasUserId()
{
    var userId =
        Guid.NewGuid();

    var deduction =
        new Deduction
        {
            Id = Guid.NewGuid(),

            Status = "Pending",

            Amount = 5000,

            Employee =
                new Employee
                {
                    UserId = userId
                }
        };

    repository
        .Setup(x =>
            x.GetDeduction(
                deduction.Id))
        .Returns(deduction);

    service.RejectDeduction(
        deduction.Id);

    notificationService.Verify(
        x =>
            x.CreateNotification(
                userId,
                "Deduction Rejected",
                It.IsAny<string>()),
        Times.Once);
}

[Test]
public void GetAllDeductions_ShouldReturnMappedDtos()
{
    repository
        .Setup(x =>
            x.GetAllDeductions())
        .Returns(
            new List<Deduction>
            {
                new Deduction
                {
                    Id = Guid.NewGuid(),

                    Amount = 5000,

                    Reason = "Late Coming",

                    DeductionMonth = 6,

                    DeductionYear = 2025,

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
        service.GetAllDeductions();

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
public void GetMyDeductions_ShouldReturnMappedDtos()
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
            x.GetEmployeeDeductions(
                employee.Id))
        .Returns(
            new List<Deduction>
            {
                new Deduction
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
        service.GetMyDeductions(
            Guid.NewGuid());

    Assert.That(
        result.Count,
        Is.EqualTo(1));
}

[Test]
public void GetMyDeductions_ShouldThrowNotFoundException_WhenEmployeeNotFound()
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
            service.GetMyDeductions(
                Guid.NewGuid()));
}


}

using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Exceptions;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.PerformanceBonusRecommendation;

namespace HRMS.Tests.Services;

[TestFixture]
public class PerformanceBonusRecommendationServiceTests
{
private Mock<IPerformanceBonusRecommendationRepository>
repository;


private PerformanceBonusRecommendationService
    service;

[SetUp]
public void Setup()
{
    repository =
        new Mock<IPerformanceBonusRecommendationRepository>();

    service =
        new PerformanceBonusRecommendationService(
            repository.Object);
}

[Test]
public void GenerateRecommendations_ShouldCreateRecommendations_WhenValid()
{
    var cycleId =
        Guid.NewGuid();

    var employee =
        new Employee
        {
            Id = Guid.NewGuid(),
            Salary = 100000
        };

    repository
        .Setup(x =>
            x.GetCycle(
                cycleId))
        .Returns(
            new PerformanceCycle());

    repository
        .Setup(x =>
            x.GetEmployeesWithReviews(
                cycleId))
        .Returns(
            new List<Employee>
            {
                employee
            });

    repository
        .Setup(x =>
            x.HasPendingRecommendation(
                employee.Id))
        .Returns(false);

    repository
        .Setup(x =>
            x.GetAverageRating(
                employee.Id,
                cycleId))
        .Returns(4.5m);

    repository
        .Setup(x =>
            x.GetMatchingRule(
                4.5m))
        .Returns(
            new PerformanceBonusRule
            {
                BonusPercentage = 20
            });

    service.GenerateRecommendations(
        cycleId);

    repository.Verify(
        x => x.AddRecommendation(
            It.IsAny<PerformanceBonusRecommendation>()),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void GenerateRecommendations_ShouldThrowNotFoundException_WhenCycleNotFound()
{
    repository
        .Setup(x =>
            x.GetCycle(
                It.IsAny<Guid>()))
        .Returns(
            (PerformanceCycle?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.GenerateRecommendations(
                Guid.NewGuid()));
}

[Test]
public void GenerateRecommendations_ShouldSkipEmployee_WhenPendingRecommendationExists()
{
    var cycleId =
        Guid.NewGuid();

    var employee =
        new Employee
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetCycle(
                cycleId))
        .Returns(
            new PerformanceCycle());

    repository
        .Setup(x =>
            x.GetEmployeesWithReviews(
                cycleId))
        .Returns(
            new List<Employee>
            {
                employee
            });

    repository
        .Setup(x =>
            x.HasPendingRecommendation(
                employee.Id))
        .Returns(true);

    service.GenerateRecommendations(
        cycleId);

    repository.Verify(
        x => x.AddRecommendation(
            It.IsAny<PerformanceBonusRecommendation>()),
        Times.Never);
}

[Test]
public void GenerateRecommendations_ShouldSkipEmployee_WhenRuleNotFound()
{
    var cycleId =
        Guid.NewGuid();

    var employee =
        new Employee
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetCycle(
                cycleId))
        .Returns(
            new PerformanceCycle());

    repository
        .Setup(x =>
            x.GetEmployeesWithReviews(
                cycleId))
        .Returns(
            new List<Employee>
            {
                employee
            });

    repository
        .Setup(x =>
            x.HasPendingRecommendation(
                employee.Id))
        .Returns(false);

    repository
        .Setup(x =>
            x.GetAverageRating(
                employee.Id,
                cycleId))
        .Returns(4);

    repository
        .Setup(x =>
            x.GetMatchingRule(
                4))
        .Returns(
            (PerformanceBonusRule?)null);

    service.GenerateRecommendations(
        cycleId);

    repository.Verify(
        x => x.AddRecommendation(
            It.IsAny<PerformanceBonusRecommendation>()),
        Times.Never);
}

[Test]
public void GenerateRecommendations_ShouldSkipEmployee_WhenBonusPercentageZero()
{
    var cycleId =
        Guid.NewGuid();

    var employee =
        new Employee
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetCycle(
                cycleId))
        .Returns(
            new PerformanceCycle());

    repository
        .Setup(x =>
            x.GetEmployeesWithReviews(
                cycleId))
        .Returns(
            new List<Employee>
            {
                employee
            });

    repository
        .Setup(x =>
            x.HasPendingRecommendation(
                employee.Id))
        .Returns(false);

    repository
        .Setup(x =>
            x.GetAverageRating(
                employee.Id,
                cycleId))
        .Returns(4);

    repository
        .Setup(x =>
            x.GetMatchingRule(
                4))
        .Returns(
            new PerformanceBonusRule
            {
                BonusPercentage = 0
            });

    service.GenerateRecommendations(
        cycleId);

    repository.Verify(
        x => x.AddRecommendation(
            It.IsAny<PerformanceBonusRecommendation>()),
        Times.Never);
}

[Test]
public void GetRecommendations_ShouldReturnMappedDtos()
{
    repository
        .Setup(x =>
            x.GetRecommendations())
        .Returns(
            new List<PerformanceBonusRecommendation>
            {
                new PerformanceBonusRecommendation
                {
                    Id = Guid.NewGuid(),

                    AverageRating = 4,

                    RecommendedPercentage = 20,

                    RecommendedAmount = 10000,

                    Status = "Pending",

                    Employee =
                        new Employee
                        {
                            FirstName = "John",
                            LastName = "Doe"
                        }
                }
            });

    var result =
        service.GetRecommendations();

    Assert.That(
        result.Count,
        Is.EqualTo(1));

    Assert.That(
        result[0].EmployeeName,
        Is.EqualTo("John Doe"));
}

[Test]
public void GetRecommendation_ShouldReturnMappedDto()
{
    var recommendation =
        new PerformanceBonusRecommendation
        {
            Id = Guid.NewGuid(),

            AverageRating = 4,

            RecommendedPercentage = 20,

            RecommendedAmount = 10000,

            Status = "Pending",

            EmployeeId =
                Guid.NewGuid(),

            Employee =
                new Employee
                {
                    FirstName = "John",
                    LastName = "Doe"
                }
        };

    repository
        .Setup(x =>
            x.GetRecommendation(
                recommendation.Id))
        .Returns(
            recommendation);

    var result =
        service.GetRecommendation(
            recommendation.Id);

    Assert.That(
        result.Id,
        Is.EqualTo(
            recommendation.Id));

    Assert.That(
        result.EmployeeName,
        Is.EqualTo(
            "John Doe"));
}

[Test]
public void GetRecommendation_ShouldThrowNotFoundException_WhenRecommendationNotFound()
{
    repository
        .Setup(x =>
            x.GetRecommendation(
                It.IsAny<Guid>()))
        .Returns(
            (PerformanceBonusRecommendation?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.GetRecommendation(
                Guid.NewGuid()));
}

[Test]
public void UpdateRecommendation_ShouldUpdateRecommendation_WhenValid()
{
    var recommendation =
        new PerformanceBonusRecommendation
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetRecommendation(
                recommendation.Id))
        .Returns(
            recommendation);

    service.UpdateRecommendation(
        recommendation.Id,
        new UpdatePerformanceBonusRecommendationDto
        {
            ApprovedAmount = 5000,
            Remarks = "Approved"
        });

    Assert.That(
        recommendation.ApprovedAmount,
        Is.EqualTo(5000));

    Assert.That(
        recommendation.Remarks,
        Is.EqualTo("Approved"));

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void UpdateRecommendation_ShouldThrowNotFoundException_WhenRecommendationNotFound()
{
    repository
        .Setup(x =>
            x.GetRecommendation(
                It.IsAny<Guid>()))
        .Returns(
            (PerformanceBonusRecommendation?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.UpdateRecommendation(
                Guid.NewGuid(),
                new UpdatePerformanceBonusRecommendationDto()));
}

[Test]
public void ApproveRecommendation_ShouldApproveRecommendation_WhenValid()
{
    var recommendation =
        new PerformanceBonusRecommendation
        {
            Id = Guid.NewGuid(),
            EmployeeId = Guid.NewGuid(),
            RecommendedAmount = 10000,
            Status = "Pending"
        };

    repository
        .Setup(x =>
            x.GetRecommendation(
                recommendation.Id))
        .Returns(
            recommendation);

    service.ApproveRecommendation(
        recommendation.Id);

    Assert.That(
        recommendation.Status,
        Is.EqualTo(
            "Approved"));

    repository.Verify(
        x => x.AddBonus(
            It.IsAny<Bonuse>()),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void ApproveRecommendation_ShouldThrowNotFoundException_WhenRecommendationNotFound()
{
    repository
        .Setup(x =>
            x.GetRecommendation(
                It.IsAny<Guid>()))
        .Returns(
            (PerformanceBonusRecommendation?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.ApproveRecommendation(
                Guid.NewGuid()));
}

[Test]
public void RejectRecommendation_ShouldRejectRecommendation_WhenValid()
{
    var recommendation =
        new PerformanceBonusRecommendation
        {
            Id = Guid.NewGuid(),
            Status = "Pending"
        };

    repository
        .Setup(x =>
            x.GetRecommendation(
                recommendation.Id))
        .Returns(
            recommendation);

    service.RejectRecommendation(
        recommendation.Id);

    Assert.That(
        recommendation.Status,
        Is.EqualTo(
            "Rejected"));

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void RejectRecommendation_ShouldThrowNotFoundException_WhenRecommendationNotFound()
{
    repository
        .Setup(x =>
            x.GetRecommendation(
                It.IsAny<Guid>()))
        .Returns(
            (PerformanceBonusRecommendation?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.RejectRecommendation(
                Guid.NewGuid()));
}


}

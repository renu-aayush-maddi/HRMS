using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Exceptions;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.PerformanceBonusRule;

namespace HRMS.Tests.Services;

[TestFixture]
public class PerformanceBonusRuleServiceTests
{
private Mock<IPerformanceBonusRuleRepository>
repository;


private PerformanceBonusRuleService
    service;

[SetUp]
public void Setup()
{
    repository =
        new Mock<IPerformanceBonusRuleRepository>();

    service =
        new PerformanceBonusRuleService(
            repository.Object);
}

[Test]
public void AddRule_ShouldAddRule_WhenValid()
{
    var dto =
        new AddPerformanceBonusRuleDto
        {
            MinRating = 3,

            MaxRating = 5,

            BonusPercentage = 20
        };

    service.AddRule(dto);

    repository.Verify(
        x => x.Add(
            It.IsAny<PerformanceBonusRule>()),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void AddRule_ShouldThrowBusinessException_WhenMinRatingGreaterThanMaxRating()
{
    var dto =
        new AddPerformanceBonusRuleDto
        {
            MinRating = 5,

            MaxRating = 3,

            BonusPercentage = 20
        };

    Assert.Throws<
        BusinessException>(
        () =>
            service.AddRule(dto));
}

[Test]
public void UpdateRule_ShouldUpdateRule_WhenValid()
{
    var rule =
        new PerformanceBonusRule
        {
            Id = Guid.NewGuid(),

            MinRating = 1,

            MaxRating = 5,

            BonusPercentage = 10,

            IsActive = true
        };

    repository
        .Setup(x =>
            x.GetById(
                rule.Id))
        .Returns(rule);

    var dto =
        new UpdatePerformanceBonusRuleDto
        {
            MinRating = 2,

            MaxRating = 5,

            BonusPercentage = 15,

            IsActive = false
        };

    service.UpdateRule(
        rule.Id,
        dto);

    Assert.That(
        rule.MinRating,
        Is.EqualTo(2));

    Assert.That(
        rule.BonusPercentage,
        Is.EqualTo(15));

    Assert.That(
        rule.IsActive,
        Is.False);

    repository.Verify(
        x => x.Update(
            rule),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void UpdateRule_ShouldThrowNotFoundException_WhenRuleNotFound()
{
    repository
        .Setup(x =>
            x.GetById(
                It.IsAny<Guid>()))
        .Returns(
            (PerformanceBonusRule?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.UpdateRule(
                Guid.NewGuid(),
                new UpdatePerformanceBonusRuleDto()));
}

[Test]
public void DeleteRule_ShouldDeleteRule_WhenValid()
{
    var rule =
        new PerformanceBonusRule
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetById(
                rule.Id))
        .Returns(rule);

    service.DeleteRule(
        rule.Id);

    repository.Verify(
        x => x.Delete(
            rule),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void DeleteRule_ShouldThrowNotFoundException_WhenRuleNotFound()
{
    repository
        .Setup(x =>
            x.GetById(
                It.IsAny<Guid>()))
        .Returns(
            (PerformanceBonusRule?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.DeleteRule(
                Guid.NewGuid()));
}

[Test]
public void GetAllRules_ShouldReturnMappedDtos()
{
    repository
        .Setup(x =>
            x.GetAll())
        .Returns(
            new List<PerformanceBonusRule>
            {
                new PerformanceBonusRule
                {
                    Id = Guid.NewGuid(),

                    MinRating = 3,

                    MaxRating = 5,

                    BonusPercentage = 20,

                    IsActive = true
                }
            });

    var result =
        service.GetAllRules();

    Assert.That(
        result.Count,
        Is.EqualTo(1));

    Assert.That(
        result[0].MinRating,
        Is.EqualTo(3));

    Assert.That(
        result[0].BonusPercentage,
        Is.EqualTo(20));
}

[Test]
public void GetRuleById_ShouldReturnMappedDto()
{
    var rule =
        new PerformanceBonusRule
        {
            Id = Guid.NewGuid(),

            MinRating = 3,

            MaxRating = 5,

            BonusPercentage = 20,

            IsActive = true
        };

    repository
        .Setup(x =>
            x.GetById(
                rule.Id))
        .Returns(rule);

    var result =
        service.GetRuleById(
            rule.Id);

    Assert.That(
        result.Id,
        Is.EqualTo(
            rule.Id));

    Assert.That(
        result.MinRating,
        Is.EqualTo(3));

    Assert.That(
        result.MaxRating,
        Is.EqualTo(5));
}

[Test]
public void GetRuleById_ShouldThrowNotFoundException_WhenRuleNotFound()
{
    repository
        .Setup(x =>
            x.GetById(
                It.IsAny<Guid>()))
        .Returns(
            (PerformanceBonusRule?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.GetRuleById(
                Guid.NewGuid()));
}


}

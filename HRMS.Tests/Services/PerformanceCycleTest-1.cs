using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Exceptions;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.PerformanceCycle;

namespace HRMS.Tests.Services;

[TestFixture]
public class PerformanceCycleServiceTests
{
private Mock<IPerformanceCycleRepository>
repository;


private PerformanceCycleService
    service;

[SetUp]
public void Setup()
{
    repository =
        new Mock<IPerformanceCycleRepository>();

    service =
        new PerformanceCycleService(
            repository.Object);
}

[Test]
public void AddCycle_ShouldAddCycle_WhenValid()
{
    var dto =
        new AddPerformanceCycleDto
        {
            Name = "2025 Annual Review",

            StartDate =
                new DateOnly(2025, 1, 1),

            EndDate =
                new DateOnly(2025, 12, 31)
        };

    repository
        .Setup(x =>
            x.CycleNameExists(
                dto.Name,
                null))
        .Returns(false);

    service.AddCycle(dto);

    repository.Verify(
        x => x.Add(
            It.IsAny<PerformanceCycle>()),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void AddCycle_ShouldThrowBusinessException_WhenStartDateGreaterThanEndDate()
{
    var dto =
        new AddPerformanceCycleDto
        {
            Name = "Cycle",

            StartDate =
                new DateOnly(2025, 12, 31),

            EndDate =
                new DateOnly(2025, 1, 1)
        };

    Assert.Throws<
        BusinessException>(
        () =>
            service.AddCycle(dto));
}

[Test]
public void AddCycle_ShouldThrowBusinessException_WhenCycleNameExists()
{
    repository
        .Setup(x =>
            x.CycleNameExists(
                It.IsAny<string>(),
                null))
        .Returns(true);

    Assert.Throws<
        BusinessException>(
        () =>
            service.AddCycle(
                new AddPerformanceCycleDto
                {
                    Name = "Existing",

                    StartDate =
                        new DateOnly(2025, 1, 1),

                    EndDate =
                        new DateOnly(2025, 12, 31)
                }));
}

[Test]
public void UpdateCycle_ShouldUpdateCycle_WhenValid()
{
    var cycle =
        new PerformanceCycle
        {
            Id = Guid.NewGuid(),

            Name = "Old Cycle"
        };

    repository
        .Setup(x =>
            x.GetById(
                cycle.Id))
        .Returns(cycle);

    repository
        .Setup(x =>
            x.CycleNameExists(
                It.IsAny<string>(),
                cycle.Id))
        .Returns(false);

    var dto =
        new UpdatePerformanceCycleDto
        {
            Name = "Updated Cycle",

            StartDate =
                new DateOnly(2025, 1, 1),

            EndDate =
                new DateOnly(2025, 12, 31),

            Status = "Open"
        };

    service.UpdateCycle(
        cycle.Id,
        dto);

    Assert.That(
        cycle.Name,
        Is.EqualTo(
            "Updated Cycle"));

    repository.Verify(
        x => x.Update(
            cycle),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void UpdateCycle_ShouldThrowNotFoundException_WhenCycleNotFound()
{
    repository
        .Setup(x =>
            x.GetById(
                It.IsAny<Guid>()))
        .Returns(
            (PerformanceCycle?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.UpdateCycle(
                Guid.NewGuid(),
                new UpdatePerformanceCycleDto()));
}

[Test]
public void UpdateCycle_ShouldThrowBusinessException_WhenStartDateGreaterThanEndDate()
{
    var cycle =
        new PerformanceCycle
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetById(
                cycle.Id))
        .Returns(cycle);

    Assert.Throws<
        BusinessException>(
        () =>
            service.UpdateCycle(
                cycle.Id,
                new UpdatePerformanceCycleDto
                {
                    Name = "Cycle",

                    StartDate =
                        new DateOnly(2025, 12, 31),

                    EndDate =
                        new DateOnly(2025, 1, 1),

                    Status = "Open"
                }));
}

[Test]
public void UpdateCycle_ShouldThrowBusinessException_WhenCycleNameExists()
{
    var cycle =
        new PerformanceCycle
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetById(
                cycle.Id))
        .Returns(cycle);

    repository
        .Setup(x =>
            x.CycleNameExists(
                It.IsAny<string>(),
                cycle.Id))
        .Returns(true);

    Assert.Throws<
        BusinessException>(
        () =>
            service.UpdateCycle(
                cycle.Id,
                new UpdatePerformanceCycleDto
                {
                    Name = "Duplicate",

                    StartDate =
                        new DateOnly(2025, 1, 1),

                    EndDate =
                        new DateOnly(2025, 12, 31),

                    Status = "Open"
                }));
}

[Test]
public void DeleteCycle_ShouldDeleteCycle_WhenValid()
{
    var cycle =
        new PerformanceCycle
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetById(
                cycle.Id))
        .Returns(cycle);

    repository
        .Setup(x =>
            x.HasReviews(
                cycle.Id))
        .Returns(false);

    repository
        .Setup(x =>
            x.HasRecommendations(
                cycle.Id))
        .Returns(false);

    service.DeleteCycle(
        cycle.Id);

    repository.Verify(
        x => x.Delete(
            cycle),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);
}

[Test]
public void DeleteCycle_ShouldThrowNotFoundException_WhenCycleNotFound()
{
    repository
        .Setup(x =>
            x.GetById(
                It.IsAny<Guid>()))
        .Returns(
            (PerformanceCycle?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.DeleteCycle(
                Guid.NewGuid()));
}

[Test]
public void DeleteCycle_ShouldThrowBusinessException_WhenCycleContainsReviews()
{
    var cycle =
        new PerformanceCycle
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetById(
                cycle.Id))
        .Returns(cycle);

    repository
        .Setup(x =>
            x.HasReviews(
                cycle.Id))
        .Returns(true);

    Assert.Throws<
        BusinessException>(
        () =>
            service.DeleteCycle(
                cycle.Id));
}

[Test]
public void DeleteCycle_ShouldThrowBusinessException_WhenCycleContainsRecommendations()
{
    var cycle =
        new PerformanceCycle
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetById(
                cycle.Id))
        .Returns(cycle);

    repository
        .Setup(x =>
            x.HasReviews(
                cycle.Id))
        .Returns(false);

    repository
        .Setup(x =>
            x.HasRecommendations(
                cycle.Id))
        .Returns(true);

    Assert.Throws<
        BusinessException>(
        () =>
            service.DeleteCycle(
                cycle.Id));
}

[Test]
public void GetAllCycles_ShouldReturnMappedDtos()
{
    repository
        .Setup(x =>
            x.GetAll())
        .Returns(
            new List<PerformanceCycle>
            {
                new PerformanceCycle
                {
                    Id = Guid.NewGuid(),

                    Name =
                        "Annual Review",

                    StartDate =
                        new DateOnly(2025,1,1),

                    EndDate =
                        new DateOnly(2025,12,31),

                    Status =
                        "Open"
                }
            });

    var result =
        service.GetAllCycles();

    Assert.That(
        result.Count,
        Is.EqualTo(1));

    Assert.That(
        result[0].Name,
        Is.EqualTo(
            "Annual Review"));
}

[Test]
public void GetCycleById_ShouldReturnMappedDto()
{
    var cycle =
        new PerformanceCycle
        {
            Id = Guid.NewGuid(),

            Name = "Annual Review",

            StartDate =
                new DateOnly(2025,1,1),

            EndDate =
                new DateOnly(2025,12,31),

            Status = "Open"
        };

    repository
        .Setup(x =>
            x.GetById(
                cycle.Id))
        .Returns(cycle);

    var result =
        service.GetCycleById(
            cycle.Id);

    Assert.That(
        result.Name,
        Is.EqualTo(
            "Annual Review"));
}

[Test]
public void GetCycleById_ShouldThrowNotFoundException_WhenCycleNotFound()
{
    repository
        .Setup(x =>
            x.GetById(
                It.IsAny<Guid>()))
        .Returns(
            (PerformanceCycle?)null);

    Assert.Throws<
        NotFoundException>(
        () =>
            service.GetCycleById(
                Guid.NewGuid()));
}


}

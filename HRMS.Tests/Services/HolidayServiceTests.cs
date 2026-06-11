using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.Holiday;
using HRMS.API.Exceptions;

namespace HRMS.Tests.Services;

[TestFixture]
public class HolidayServiceTests
{
    [Test]
    public void GetAll_ShouldReturnMappedDtos()
    {
        var repository =
            new Mock<IHolidayRepository>();

        repository
            .Setup(x => x.GetAll())
            .Returns(
                new List<Holiday>
                {
                    new Holiday
                    {
                        Id = Guid.NewGuid(),
                        Name = "Christmas",
                        HolidayDate =
                            new DateOnly(
                                2026,
                                12,
                                25),
                        Description =
                            "Holiday",
                        IsOptional =
                            false
                    }
                });

        var service =
            new HolidayService(
                repository.Object);

        var result =
            service.GetAll();

        Assert.That(
            result.Count,
            Is.EqualTo(1));

        Assert.That(
            result[0].Name,
            Is.EqualTo(
                "Christmas"));
    }

    [Test]
    public void GetUpcoming_ShouldReturnMappedDtos()
    {
        var repository =
            new Mock<IHolidayRepository>();

        repository
            .Setup(x => x.GetUpcoming())
            .Returns(
                new List<Holiday>
                {
                    new Holiday
                    {
                        Id = Guid.NewGuid(),
                        Name = "New Year",
                        HolidayDate =
                            new DateOnly(
                                2027,
                                1,
                                1)
                    }
                });

        var service =
            new HolidayService(
                repository.Object);

        var result =
            service.GetUpcoming();

        Assert.That(
            result.Count,
            Is.EqualTo(1));

        Assert.That(
            result[0].Name,
            Is.EqualTo(
                "New Year"));
    }

    [Test]
    public void AddHoliday_ShouldAddHoliday_WhenValid()
    {
        var repository =
            new Mock<IHolidayRepository>();

        repository
            .Setup(x =>
                x.HolidayExists(
                    It.IsAny<string>(),
                    It.IsAny<DateOnly>()))
            .Returns(false);

        var service =
            new HolidayService(
                repository.Object);

        service.AddHoliday(
            new AddHolidayDto
            {
                Name =
                    "Christmas",

                HolidayDate =
                    DateOnly.FromDateTime(
                        DateTime.Today.AddDays(30)),

                Description =
                    "Festival",

                IsOptional =
                    false
            });

        repository.Verify(
            x => x.Add(
                It.IsAny<Holiday>()),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void AddHoliday_ShouldThrowBusinessException_WhenDuplicateHoliday()
    {
        var repository =
            new Mock<IHolidayRepository>();

        repository
            .Setup(x =>
                x.HolidayExists(
                    It.IsAny<string>(),
                    It.IsAny<DateOnly>()))
            .Returns(true);

        var service =
            new HolidayService(
                repository.Object);

        Assert.Throws<
            BusinessException>(
            () =>
            service.AddHoliday(
                new AddHolidayDto
                {
                    Name =
                        "Christmas",

                    HolidayDate =
                        new DateOnly(
                            2026,
                            12,
                            25)
                }));
    }

    [Test]
    public void AddHoliday_ShouldThrowBusinessException_WhenDateInvalid()
    {
        var repository =
            new Mock<IHolidayRepository>();

        repository
            .Setup(x =>
                x.HolidayExists(
                    It.IsAny<string>(),
                    It.IsAny<DateOnly>()))
            .Returns(false);

        var service =
            new HolidayService(
                repository.Object);

        Assert.Throws<
            BusinessException>(
            () =>
            service.AddHoliday(
                new AddHolidayDto
                {
                    Name =
                        "Old Holiday",

                    HolidayDate =
                        new DateOnly(
                            2020,
                            1,
                            1)
                }));
    }

    [Test]
    public void UpdateHoliday_ShouldUpdateHoliday_WhenValid()
    {
        var repository =
            new Mock<IHolidayRepository>();

        var holiday =
            new Holiday
            {
                Id = Guid.NewGuid(),
                Name = "Old Name"
            };

        repository
            .Setup(x =>
                x.GetById(
                    holiday.Id))
            .Returns(holiday);

        var service =
            new HolidayService(
                repository.Object);

        service.UpdateHoliday(
            holiday.Id,
            new UpdateHolidayDto
            {
                Name =
                    "Christmas",

                HolidayDate =
                    new DateOnly(
                        2026,
                        12,
                        25),

                Description =
                    "Updated",

                IsOptional =
                    false
            });

        Assert.That(
            holiday.Name,
            Is.EqualTo(
                "Christmas"));

        repository.Verify(
            x => x.Update(
                holiday),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void UpdateHoliday_ShouldThrowNotFoundException_WhenHolidayNotFound()
    {
        var repository =
            new Mock<IHolidayRepository>();

        repository
            .Setup(x =>
                x.GetById(
                    It.IsAny<Guid>()))
            .Returns((Holiday?)null);

        var service =
            new HolidayService(
                repository.Object);

        Assert.Throws<
            NotFoundException>(
            () =>
            service.UpdateHoliday(
                Guid.NewGuid(),
                new UpdateHolidayDto()));
    }

    [Test]
    public void DeleteHoliday_ShouldDeleteHoliday_WhenValid()
    {
        var repository =
            new Mock<IHolidayRepository>();

        var holiday =
            new Holiday
            {
                Id = Guid.NewGuid(),
                Name = "Christmas"
            };

        repository
            .Setup(x =>
                x.GetById(
                    holiday.Id))
            .Returns(holiday);

        var service =
            new HolidayService(
                repository.Object);

        service.DeleteHoliday(
            holiday.Id);

        repository.Verify(
            x => x.Delete(
                holiday),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void DeleteHoliday_ShouldThrowNotFoundException_WhenHolidayNotFound()
    {
        var repository =
            new Mock<IHolidayRepository>();

        repository
            .Setup(x =>
                x.GetById(
                    It.IsAny<Guid>()))
            .Returns((Holiday?)null);

        var service =
            new HolidayService(
                repository.Object);

        Assert.Throws<
            NotFoundException>(
            () =>
            service.DeleteHoliday(
                Guid.NewGuid()));
    }
}
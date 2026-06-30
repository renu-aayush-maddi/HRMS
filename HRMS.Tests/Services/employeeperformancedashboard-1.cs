using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Exceptions;
using HRMS.API.Models.DTOs.Dashboard;
using HRMS.API.Models.Entities;

namespace HRMS.Tests.Services;

[TestFixture]
public class PerformanceDashboardServiceTests
{
    private Mock<IPerformanceDashboardRepository>
        repository;

    private PerformanceDashboardService
        service;

    [SetUp]
    public void Setup()
    {
        repository =
            new Mock<IPerformanceDashboardRepository>();

        service =
            new PerformanceDashboardService(
                repository.Object);
    }

    [Test]
    public void GetManagerDashboard_ShouldReturnDashboard_WhenManagerExists()
    {
        var manager =
            new Employee
            {
                Id = Guid.NewGuid()
            };

        var cycleId =
            Guid.NewGuid();

        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    manager.Id))
            .Returns(manager);

        repository
            .Setup(x =>
                x.GetTeamMembersCount(
                    manager.Id))
            .Returns(5);

        repository
            .Setup(x =>
                x.GetTotalGoals(
                    manager.Id))
            .Returns(20);

        repository
            .Setup(x =>
                x.GetCompletedGoals(
                    manager.Id))
            .Returns(15);

        repository
            .Setup(x =>
                x.GetAverageRating(
                    manager.Id,
                    cycleId))
            .Returns(4.5m);

        repository
            .Setup(x =>
                x.GetTopPerformer(
                    manager.Id,
                    cycleId))
            .Returns("John Doe");

        var result =
            service.GetManagerDashboard(
                manager.Id,
                cycleId);

        Assert.That(
            result.TeamMembers,
            Is.EqualTo(5));

        Assert.That(
            result.TotalGoals,
            Is.EqualTo(20));

        Assert.That(
            result.CompletedGoals,
            Is.EqualTo(15));

        Assert.That(
            result.PendingGoals,
            Is.EqualTo(5));

        Assert.That(
            result.AverageRating,
            Is.EqualTo(4.5m));

        Assert.That(
            result.TopPerformer,
            Is.EqualTo("John Doe"));
    }

    [Test]
    public void GetManagerDashboard_ShouldThrowNotFoundException_WhenManagerNotFound()
    {
        repository
            .Setup(x =>
                x.GetEmployeeByUserId(
                    It.IsAny<Guid>()))
            .Returns(
                (Employee?)null);

        Assert.Throws<NotFoundException>(
            () =>
                service.GetManagerDashboard(
                    Guid.NewGuid(),
                    Guid.NewGuid()));
    }

    [Test]
    public void GetManagerDashboard_ShouldCalculatePendingGoalsCorrectly()
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
                x.GetTotalGoals(
                    manager.Id))
            .Returns(30);

        repository
            .Setup(x =>
                x.GetCompletedGoals(
                    manager.Id))
            .Returns(12);

        repository
            .Setup(x =>
                x.GetTeamMembersCount(
                    manager.Id))
            .Returns(5);

        repository
            .Setup(x =>
                x.GetAverageRating(
                    manager.Id,
                    It.IsAny<Guid>()))
            .Returns(4);

        repository
            .Setup(x =>
                x.GetTopPerformer(
                    manager.Id,
                    It.IsAny<Guid>()))
            .Returns("John");

        var result =
            service.GetManagerDashboard(
                manager.Id,
                Guid.NewGuid());

        Assert.That(
            result.PendingGoals,
            Is.EqualTo(18));
    }

    [Test]
    public void GetManagerDashboard_ShouldReturnNA_WhenTopPerformerIsNull()
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
                x.GetTeamMembersCount(
                    manager.Id))
            .Returns(5);

        repository
            .Setup(x =>
                x.GetTotalGoals(
                    manager.Id))
            .Returns(10);

        repository
            .Setup(x =>
                x.GetCompletedGoals(
                    manager.Id))
            .Returns(5);

        repository
            .Setup(x =>
                x.GetAverageRating(
                    manager.Id,
                    It.IsAny<Guid>()))
            .Returns(4);

        repository
            .Setup(x =>
                x.GetTopPerformer(
                    manager.Id,
                    It.IsAny<Guid>()))
            .Returns(
                (string?)null);

        var result =
            service.GetManagerDashboard(
                manager.Id,
                Guid.NewGuid());

        Assert.That(
            result.TopPerformer,
            Is.EqualTo("N/A"));
    }

    [Test]
    public void GetHrDashboard_ShouldReturnDashboard()
    {
        var cycleId =
            Guid.NewGuid();

        var topPerformers =
            new List<EmployeePerformanceDto>
            {
                new()
            };

        var lowestPerformers =
            new List<EmployeePerformanceDto>
            {
                new()
            };

        var departmentRatings =
            new List<DepartmentRatingDto>
            {
                new()
            };

        repository
            .Setup(x =>
                x.GetTotalEmployees())
            .Returns(100);

        repository
            .Setup(x =>
                x.GetTotalReviews(
                    cycleId))
            .Returns(90);

        repository
            .Setup(x =>
                x.GetCompanyAverageRating(
                    cycleId))
            .Returns(4.2m);

        repository
            .Setup(x =>
                x.GetReviewCompletionPercentage(
                    cycleId))
            .Returns(90m);

        repository
            .Setup(x =>
                x.GetTopPerformers(
                    cycleId))
            .Returns(topPerformers);

        repository
            .Setup(x =>
                x.GetLowestPerformers(
                    cycleId))
            .Returns(lowestPerformers);

        repository
            .Setup(x =>
                x.GetDepartmentRatings(
                    cycleId))
            .Returns(departmentRatings);

        var result =
            service.GetHrDashboard(
                cycleId);

        Assert.That(
            result.TotalEmployees,
            Is.EqualTo(100));

        Assert.That(
            result.TotalReviews,
            Is.EqualTo(90));

        Assert.That(
            result.AverageRating,
            Is.EqualTo(4.2m));

        Assert.That(
            result.ReviewCompletionPercentage,
            Is.EqualTo(90m));

        Assert.That(
            result.TopPerformers.Count,
            Is.EqualTo(1));

        Assert.That(
            result.LowestPerformers.Count,
            Is.EqualTo(1));

        Assert.That(
            result.DepartmentRatings.Count,
            Is.EqualTo(1));
    }
}
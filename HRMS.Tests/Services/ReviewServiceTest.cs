using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Review;
using HRMS.API.Models.Entities;

namespace HRMS.Tests.Services;

[TestFixture]
public class ReviewServiceTests
{
private Mock<IReviewRepository>
repository;


private Mock<INotificationService>
    notificationService;

private ReviewService
    service;

[SetUp]
public void Setup()
{
    repository =
        new Mock<IReviewRepository>();

    notificationService =
        new Mock<INotificationService>();

    service =
        new ReviewService(
            repository.Object,
            notificationService.Object);
}

[Test]
public void AddReview_ShouldAddReview_WhenValid()
{
    var reviewerUserId =
        Guid.NewGuid();

    var employee =
        new Employee
        {
            Id = Guid.NewGuid(),
            ManagerId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

    var reviewer =
        new Employee
        {
            Id = employee.ManagerId.Value
        };

    var cycle =
        new PerformanceCycle
        {
            Id = Guid.NewGuid()
        };

    repository
        .Setup(x =>
            x.GetCycle(
                cycle.Id))
        .Returns(cycle);

    repository
        .Setup(x =>
            x.GetEmployee(
                employee.Id))
        .Returns(employee);

    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                reviewerUserId))
        .Returns(reviewer);

    var dto =
        new AddReviewDto
        {
            EmployeeId =
                employee.Id,

            PerformanceCycleId =
                cycle.Id,

            Rating = 4,

            Comments =
                "Good work"
        };

    service.AddReview(
        reviewerUserId,
        dto);

    repository.Verify(
        x => x.AddReview(
            It.IsAny<PerformanceReview>()),
        Times.Once);

    repository.Verify(
        x => x.SaveChanges(),
        Times.Once);

    notificationService.Verify(
        x =>
            x.CreateNotification(
                employee.UserId.Value,
                "Performance Review Added",
                It.IsAny<string>()),
        Times.Once);
}

[Test]
public void AddReview_ShouldThrowException_WhenCycleNotFound()
{
    repository
        .Setup(x =>
            x.GetCycle(
                It.IsAny<Guid>()))
        .Returns(
            (PerformanceCycle?)null);

    Assert.Throws<Exception>(
        () =>
            service.AddReview(
                Guid.NewGuid(),
                new AddReviewDto
                {
                    EmployeeId =
                        Guid.NewGuid(),

                    PerformanceCycleId =
                        Guid.NewGuid(),

                    Rating = 4
                }));
}

[Test]
public void AddReview_ShouldThrowException_WhenEmployeeNotFound()
{
    repository
        .Setup(x =>
            x.GetCycle(
                It.IsAny<Guid>()))
        .Returns(
            new PerformanceCycle());

    repository
        .Setup(x =>
            x.GetEmployee(
                It.IsAny<Guid>()))
        .Returns(
            (Employee?)null);

    Assert.Throws<Exception>(
        () =>
            service.AddReview(
                Guid.NewGuid(),
                new AddReviewDto
                {
                    EmployeeId =
                        Guid.NewGuid(),

                    PerformanceCycleId =
                        Guid.NewGuid(),

                    Rating = 4
                }));
}

[Test]
public void AddReview_ShouldThrowException_WhenReviewerNotFound()
{
    repository
        .Setup(x =>
            x.GetCycle(
                It.IsAny<Guid>()))
        .Returns(
            new PerformanceCycle());

    repository
        .Setup(x =>
            x.GetEmployee(
                It.IsAny<Guid>()))
        .Returns(
            new Employee());

    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns(
            (Employee?)null);

    Assert.Throws<Exception>(
        () =>
            service.AddReview(
                Guid.NewGuid(),
                new AddReviewDto
                {
                    EmployeeId =
                        Guid.NewGuid(),

                    PerformanceCycleId =
                        Guid.NewGuid(),

                    Rating = 4
                }));
}

[Test]
public void AddReview_ShouldThrowException_WhenEmployeeNotInManagersTeam()
{
    repository
        .Setup(x =>
            x.GetCycle(
                It.IsAny<Guid>()))
        .Returns(
            new PerformanceCycle());

    repository
        .Setup(x =>
            x.GetEmployee(
                It.IsAny<Guid>()))
        .Returns(
            new Employee
            {
                ManagerId =
                    Guid.NewGuid()
            });

    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns(
            new Employee
            {
                Id =
                    Guid.NewGuid()
            });

    Assert.Throws<Exception>(
        () =>
            service.AddReview(
                Guid.NewGuid(),
                new AddReviewDto
                {
                    EmployeeId =
                        Guid.NewGuid(),

                    PerformanceCycleId =
                        Guid.NewGuid(),

                    Rating = 4
                }));
}

[Test]
public void AddReview_ShouldThrowException_WhenRatingLessThanOne()
{
    var managerId =
        Guid.NewGuid();

    repository
        .Setup(x =>
            x.GetCycle(
                It.IsAny<Guid>()))
        .Returns(
            new PerformanceCycle());

    repository
        .Setup(x =>
            x.GetEmployee(
                It.IsAny<Guid>()))
        .Returns(
            new Employee
            {
                ManagerId =
                    managerId
            });

    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns(
            new Employee
            {
                Id = managerId
            });

    Assert.Throws<Exception>(
        () =>
            service.AddReview(
                Guid.NewGuid(),
                new AddReviewDto
                {
                    EmployeeId =
                        Guid.NewGuid(),

                    PerformanceCycleId =
                        Guid.NewGuid(),

                    Rating = 0
                }));
}

[Test]
public void AddReview_ShouldThrowException_WhenRatingGreaterThanFive()
{
    var managerId =
        Guid.NewGuid();

    repository
        .Setup(x =>
            x.GetCycle(
                It.IsAny<Guid>()))
        .Returns(
            new PerformanceCycle());

    repository
        .Setup(x =>
            x.GetEmployee(
                It.IsAny<Guid>()))
        .Returns(
            new Employee
            {
                ManagerId =
                    managerId
            });

    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns(
            new Employee
            {
                Id = managerId
            });

    Assert.Throws<Exception>(
        () =>
            service.AddReview(
                Guid.NewGuid(),
                new AddReviewDto
                {
                    EmployeeId =
                        Guid.NewGuid(),

                    PerformanceCycleId =
                        Guid.NewGuid(),

                    Rating = 6
                }));
}

[Test]
public void GetAllReviews_ShouldReturnMappedDtos()
{
    repository
        .Setup(x =>
            x.GetAllReviews())
        .Returns(
            new List<PerformanceReview>
            {
                new PerformanceReview
                {
                    Id = Guid.NewGuid(),

                    Rating = 4,

                    Employee =
                        new Employee
                        {
                            FirstName = "John",
                            LastName = "Doe"
                        },

                    Reviewer =
                        new Employee
                        {
                            FirstName = "Manager",
                            LastName = "One"
                        }
                }
            });

    var result =
        service.GetAllReviews();

    Assert.That(
        result.Count,
        Is.EqualTo(1));

    Assert.That(
        result[0].EmployeeName,
        Is.EqualTo("John Doe"));

    Assert.That(
        result[0].ReviewerName,
        Is.EqualTo("Manager One"));
}

[Test]
public void GetEmployeeReviews_ShouldReturnMappedDtos()
{
    repository
        .Setup(x =>
            x.GetEmployeeReviews(
                It.IsAny<Guid>()))
        .Returns(
            new List<PerformanceReview>
            {
                new PerformanceReview
                {
                    Employee =
                        new Employee
                        {
                            FirstName = "John",
                            LastName = "Doe"
                        },

                    Reviewer =
                        new Employee
                        {
                            FirstName = "Manager",
                            LastName = "One"
                        }
                }
            });

    var result =
        service.GetEmployeeReviews(
            Guid.NewGuid());

    Assert.That(
        result.Count,
        Is.EqualTo(1));
}

[Test]
public void GetMyReviews_ShouldReturnMappedDtos()
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
            x.GetEmployeeReviews(
                employee.Id))
        .Returns(
            new List<PerformanceReview>
            {
                new PerformanceReview
                {
                    Employee =
                        new Employee
                        {
                            FirstName = "John",
                            LastName = "Doe"
                        },

                    Reviewer =
                        new Employee
                        {
                            FirstName = "Manager",
                            LastName = "One"
                        }
                }
            });

    var result =
        service.GetMyReviews(
            Guid.NewGuid());

    Assert.That(
        result.Count,
        Is.EqualTo(1));
}

[Test]
public void GetMyReviews_ShouldThrowException_WhenEmployeeNotFound()
{
    repository
        .Setup(x =>
            x.GetEmployeeByUserId(
                It.IsAny<Guid>()))
        .Returns(
            (Employee?)null);

    Assert.Throws<Exception>(
        () =>
            service.GetMyReviews(
                Guid.NewGuid()));
}


}

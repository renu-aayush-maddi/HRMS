using Moq;
using NUnit.Framework;

using HRMS.API.Services;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.Notification;

namespace HRMS.Tests.Services;

[TestFixture]
public class NotificationServiceTests
{
    private Mock<INotificationRepository>
        repository;

    private NotificationService
        service;

    [SetUp]
    public void Setup()
    {
        repository =
            new Mock<INotificationRepository>();

        service =
            new NotificationService(
                repository.Object);
    }

    [Test]
    public void CreateNotification_ShouldAddNotification()
    {
        service.CreateNotification(
            Guid.NewGuid(),
            "Test Title",
            "Test Message");

        repository.Verify(
            x => x.AddNotification(
                It.IsAny<Notification>()),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void GetNotifications_ShouldReturnMappedDtos()
    {
        var userId =
            Guid.NewGuid();

        repository
            .Setup(x =>
                x.GetUserNotifications(
                    userId))
            .Returns(
            [
                new Notification
                {
                    Id = Guid.NewGuid(),

                    Title = "Title",

                    Message = "Message",

                    IsRead = false,

                    CreatedAt = DateTime.UtcNow
                }
            ]);

        var result =
            service.GetNotifications(
                userId);

        Assert.That(
            result.Count,
            Is.EqualTo(1));

        Assert.That(
            result[0].Title,
            Is.EqualTo("Title"));

        Assert.That(
            result[0].Message,
            Is.EqualTo("Message"));

        Assert.That(
            result[0].IsRead,
            Is.False);
    }

    [Test]
    public void GetNotifications_ShouldReturnEmptyList_WhenNoNotificationsExist()
    {
        repository
            .Setup(x =>
                x.GetUserNotifications(
                    It.IsAny<Guid>()))
            .Returns([]);

        var result =
            service.GetNotifications(
                Guid.NewGuid());

        Assert.That(
            result,
            Is.Empty);
    }

    [Test]
    public void MarkAsRead_ShouldMarkNotificationAsRead()
    {
        var notification =
            new Notification
            {
                Id = Guid.NewGuid(),

                IsRead = false
            };

        repository
            .Setup(x =>
                x.GetNotification(
                    notification.Id))
            .Returns(
                notification);

        service.MarkAsRead(
            notification.Id);

        Assert.That(
            notification.IsRead,
            Is.True);

        repository.Verify(
            x => x.UpdateNotification(
                notification),
            Times.Once);

        repository.Verify(
            x => x.SaveChanges(),
            Times.Once);
    }

    [Test]
    public void MarkAsRead_ShouldThrowException_WhenNotificationNotFound()
    {
        repository
            .Setup(x =>
                x.GetNotification(
                    It.IsAny<Guid>()))
            .Returns(
                (Notification?)null);

        Assert.Throws<Exception>(
            () =>
                service.MarkAsRead(
                    Guid.NewGuid()));
    }
}
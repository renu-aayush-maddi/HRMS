using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Notification;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

public class NotificationService
    : INotificationService
{
    private readonly INotificationRepository repository;

    public NotificationService(
        INotificationRepository repository)
    {
        this.repository = repository;
    }

    public void CreateNotification(
        Guid userId,
        string title,
        string message)
    {
        Notification notification =
            new Notification
            {
                Id = Guid.NewGuid(),

                UserId = userId,

                Title = title,

                Message = message,

                IsRead = false
            };

        repository.AddNotification(notification);

        repository.SaveChanges();
    }

    public List<NotificationResponseDto>
        GetNotifications(Guid userId)
    {
        return repository
            .GetUserNotifications(userId)
            .Select(n => new NotificationResponseDto
            {
                Id = n.Id,

                Title = n.Title,

                Message = n.Message,

                IsRead = n.IsRead,

                CreatedAt = n.CreatedAt
            })
            .ToList();
    }

    public void MarkAsRead(Guid notificationId)
    {
        var notification =
            repository.GetNotification(notificationId);

        if(notification == null)
        {
            throw new Exception(
                "Notification not found");
        }

        notification.IsRead = true;

        repository.UpdateNotification(notification);

        repository.SaveChanges();
    }
}
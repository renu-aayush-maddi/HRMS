using HRMS.API.Models.DTOs.Notification;

namespace HRMS.API.Interfaces;

public interface INotificationService
{
    List<NotificationResponseDto>
        GetNotifications(Guid userId);

    void MarkAsRead(Guid notificationId);

    void CreateNotification(
        Guid userId,
        string title,
        string message);
}
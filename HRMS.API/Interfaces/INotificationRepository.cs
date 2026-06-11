using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface INotificationRepository
{
    void AddNotification(Notification notification);

    List<Notification> GetUserNotifications(Guid userId);

    Notification? GetNotification(Guid id);

    void UpdateNotification(Notification notification);

    void SaveChanges();
}
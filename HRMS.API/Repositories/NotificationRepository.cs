using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;

namespace HRMS.API.Repositories;

public class NotificationRepository
    : INotificationRepository
{
    private readonly AppDbContext context;

    public NotificationRepository(
        AppDbContext context)
    {
        this.context = context;
    }

    public void AddNotification(
        Notification notification)
    {
        context.Notifications.Add(notification);
    }

    public List<Notification>
        GetUserNotifications(Guid userId)
    {
        return context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToList();
    }

    public Notification? GetNotification(Guid id)
    {
        return context.Notifications
            .FirstOrDefault(n => n.Id == id);
    }

    public void UpdateNotification(
        Notification notification)
    {
        context.Notifications.Update(notification);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}
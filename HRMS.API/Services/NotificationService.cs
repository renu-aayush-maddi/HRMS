using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Notification;
using HRMS.API.Models.Entities;
using HRMS.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HRMS.API.Services;

public class NotificationService
    : INotificationService
{
    private readonly INotificationRepository repository;
    private readonly IHubContext<NotificationHub> hubContext;

    public NotificationService(
        INotificationRepository repository,
        IHubContext<NotificationHub> hubContext)
    {
        this.repository = repository;
        this.hubContext = hubContext;
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

                IsRead = false,

                CreatedAt = DateTime.Now
            };

        repository.AddNotification(notification);

        repository.SaveChanges();

        // Push real-time notification via SignalR (fire-and-forget)
        _ = hubContext.Clients.Group(userId.ToString()).SendAsync("ReceiveNotification", new NotificationResponseDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        });
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
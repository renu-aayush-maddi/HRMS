using HRMS.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/notifications")]
[ApiController]
[Authorize]
public class NotificationsController: ControllerBase
{
    private readonly INotificationService service;

    public NotificationsController(INotificationService service)
    {
        this.service = service;
    }

    [HttpGet("{userId}")]
    public IActionResult GetNotifications(Guid userId)
    {
        return Ok(service.GetNotifications(userId));
    }

    [HttpPut("{notificationId}/read")]
    public IActionResult MarkAsRead(Guid notificationId)
    {
        service.MarkAsRead(notificationId);

        return Ok(new{message = "Notification marked as read"});
    }
}
namespace HRMS.API.Models.DTOs.Notification;
public class NotificationResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Message { get; set; }
    public bool? IsRead { get; set; }
    public DateTime? CreatedAt { get; set; }
}

namespace HRMS.API.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(
        string action,
        string entityType,
        Guid entityId,
        string details,
        CancellationToken cancellationToken = default);
}
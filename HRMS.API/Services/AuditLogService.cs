using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository repository;
    private readonly IUserContextService userContextService;

    public AuditLogService(IAuditLogRepository repository, IUserContextService userContextService)
    {
        this.repository = repository;
        this.userContextService = userContextService;
    }

    public async Task LogAsync(string action, string entityType, Guid entityId, string details, CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            PerformedBy = userContextService.GetUserId(),
            PerformedAt = DateTime.Now
        };

        await repository.AddAsync(auditLog, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog auditLog,CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
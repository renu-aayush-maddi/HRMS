using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;

namespace HRMS.API.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext context;

    public AuditLogRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task AddAsync(AuditLog auditLog, CancellationToken cancellationToken = default)
    {
        await context.AuditLogs.AddAsync(auditLog, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
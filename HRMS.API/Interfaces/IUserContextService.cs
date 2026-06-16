using System.Security.Claims;

namespace HRMS.API.Interfaces;

public interface IUserContextService
{
    Guid GetUserId();

    string GetRole();

    bool IsAdminOrHr();

    Task<Guid?> GetEmployeeIdAsync(CancellationToken cancellationToken = default);
}
using System.Security.Claims;
using HRMS.API.Data;
using HRMS.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly AppDbContext _context;

    public UserContextService(
        IHttpContextAccessor httpContextAccessor,
        AppDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public Guid GetUserId()
    {
        var userIdClaim =
            _httpContextAccessor
                .HttpContext?
                .User
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user.");
        }

        return userId;
    }

    public string GetRole()
    {
        return _httpContextAccessor
                   .HttpContext?
                   .User
                   .FindFirst(ClaimTypes.Role)?
                   .Value
               ?? string.Empty;
    }

    public bool IsAdminOrHr()
    {
        var role = GetRole();

        return role == "Admin"
               || role == "HR";
    }

    public async Task<Guid?> GetEmployeeIdAsync(CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();

        return await _context.Employees
            .Where(e =>
                e.UserId == userId &&
                !e.IsDeleted)
            .Select(e =>
                (Guid?)e.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
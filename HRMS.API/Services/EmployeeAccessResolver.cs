using HRMS.API.Exceptions;
using HRMS.API.Interfaces;

namespace HRMS.API.Services;

public class EmployeeAccessResolver: IEmployeeAccessResolver
{
    private readonly IUserContextService _userContextService;

    public EmployeeAccessResolver(IUserContextService userContextService)
    {
        _userContextService = userContextService;
    }

    public async Task<Guid> ResolveEmployeeIdAsync(Guid? requestedEmployeeId,CancellationToken cancellationToken = default)
    {
        if (_userContextService.IsAdminOrHr())
        {
            if (requestedEmployeeId == null)
            {
                throw new ValidationException("EmployeeId is required.");
            }

            return requestedEmployeeId.Value;
        }

        var employeeId =
            await _userContextService
                .GetEmployeeIdAsync(
                    cancellationToken);

        if (employeeId == null)
        {
            throw new NotFoundException("Employee profile not found.");
        }

        return employeeId.Value;
    }

    public async Task ValidateEmployeeOwnershipAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        if (_userContextService.IsAdminOrHr())
        {
            return;
        }

        var currentEmployeeId =
            await _userContextService
                .GetEmployeeIdAsync(
                    cancellationToken);

        if (currentEmployeeId != employeeId)
        {
            throw new ForbiddenException("You are not authorized to access this employee.");
        }
    }
}
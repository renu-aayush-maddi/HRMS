namespace HRMS.API.Interfaces;

public interface IEmployeeAccessResolver
{
    Task<Guid> ResolveEmployeeIdAsync(
        Guid? requestedEmployeeId,
        CancellationToken cancellationToken = default);

    Task ValidateEmployeeOwnershipAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default);
}
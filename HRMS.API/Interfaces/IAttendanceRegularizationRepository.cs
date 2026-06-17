using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IAttendanceRegularizationRepository
{
    Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<AttendanceLog?> GetAttendanceAsync(Guid employeeId, DateOnly attendanceDate, CancellationToken cancellationToken = default);

    Task<AttendanceRegularization?> GetRegularizationAsync(Guid regularizationId, CancellationToken cancellationToken = default);

    IQueryable<AttendanceRegularization> GetRegularizations();

    Task<bool> PendingRequestExistsAsync(Guid employeeId, DateOnly attendanceDate, CancellationToken cancellationToken = default);

    Task AddRegularizationAsync(AttendanceRegularization regularization, CancellationToken cancellationToken = default);

    void UpdateRegularization(AttendanceRegularization regularization);

    void UpdateAttendance(AttendanceLog attendance);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
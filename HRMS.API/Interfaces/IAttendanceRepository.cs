using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IAttendanceRepository
{
    Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<Employee?> GetEmployeeByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<AttendanceLog?> GetAttendanceAsync(Guid attendanceId, CancellationToken cancellationToken = default);

    Task<AttendanceLog?> GetTodayAttendanceAsync(Guid employeeId, CancellationToken cancellationToken = default);

    IQueryable<AttendanceLog> GetAttendances();

    Task AddAttendanceAsync(AttendanceLog attendance, CancellationToken cancellationToken = default);

    void UpdateAttendance(AttendanceLog attendance);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
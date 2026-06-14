using HRMS.API.Models.DTOs.Attendance;
using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IAttendanceRepository
{
    Task<AttendanceLog?> GetTodayAttendanceAsync(Guid employeeId);

    Task<List<AttendanceLog>> GetAttendanceAsync(AttendanceQueryDto query, int skip, int take);

    Task<int> GetAttendanceCountAsync(AttendanceQueryDto query);

    Task<List<AttendanceLog>> GetEmployeeAttendanceAsync(Guid employeeId);

    Task AddAttendanceAsync(AttendanceLog attendance);

    void UpdateAttendance(AttendanceLog attendance);

    Task<Employee?> GetEmployeeAsync(Guid employeeId);

    Task<Employee?> GetEmployeeByUserIdAsync(Guid userId);

    Task SaveChangesAsync();
}
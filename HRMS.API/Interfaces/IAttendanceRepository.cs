using HRMS.API.Models.Entities;
using HRMS.API.Models.DTOs.Attendance;

namespace HRMS.API.Interfaces;

public interface IAttendanceRepository
{
    AttendanceLog? GetTodayAttendance(Guid employeeId);

    List<AttendanceLog> GetAttendance(AttendanceQueryDto query,int skip,int take);

    int GetAttendanceCount(AttendanceQueryDto query);

    List<AttendanceLog> GetEmployeeAttendance(Guid employeeId);

    void AddAttendance(AttendanceLog attendance);

    void UpdateAttendance(AttendanceLog attendance);

    Employee? GetEmployee(Guid employeeId);

    Employee? GetEmployeeByUserId(Guid userId);

    void SaveChanges();
}
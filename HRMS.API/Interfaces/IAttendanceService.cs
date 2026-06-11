using HRMS.API.Models.DTOs.Attendance;
using HRMS.API.Models.DTOs.Common;


namespace HRMS.API.Interfaces;

public interface IAttendanceService
{

    void CheckIn(Guid userId);

    void CheckOut(Guid userId);

    PaginatedResponseDto<AttendanceResponseDto> GetAttendance(AttendanceQueryDto query);    
    
    List<AttendanceResponseDto> GetEmployeeAttendance(Guid employeeId);
}
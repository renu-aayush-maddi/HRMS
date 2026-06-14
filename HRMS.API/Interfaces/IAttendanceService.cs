using HRMS.API.Models.DTOs.Attendance;
using HRMS.API.Models.DTOs.Common;

namespace HRMS.API.Interfaces;

public interface IAttendanceService
{
    Task CheckInAsync(Guid userId);

    Task CheckOutAsync(Guid userId);

    Task<PaginatedResponseDto<AttendanceResponseDto>> GetAttendanceAsync(AttendanceQueryDto query);

    Task<List<AttendanceResponseDto>>GetEmployeeAttendanceAsync(Guid employeeId);
}
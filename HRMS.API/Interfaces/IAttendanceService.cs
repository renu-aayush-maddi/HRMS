using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.Attendance;

namespace HRMS.API.Interfaces;

public interface IAttendanceService
{
    Task CheckInAsync(CancellationToken cancellationToken = default);

    Task CheckOutAsync(CancellationToken cancellationToken = default);

    Task<PagedResponse<AttendanceResponseDto>> GetAttendanceAsync(AttendanceFilterDto filter, CancellationToken cancellationToken = default);

    Task<PagedResponse<AttendanceResponseDto>> GetEmployeeAttendanceAsync(AttendanceFilterDto filter, CancellationToken cancellationToken = default);

    Task<byte[]> ExportAttendanceAsync(AttendanceFilterDto filter, CancellationToken cancellationToken = default);
}
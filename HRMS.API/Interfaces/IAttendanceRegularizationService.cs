using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.AttendanceRegularization;

namespace HRMS.API.Interfaces;

public interface IAttendanceRegularizationService
{
    Task<AttendanceRegularizationResponseDto> CreateRequestAsync(CreateAttendanceRegularizationDto dto, CancellationToken cancellationToken = default);

    Task<PagedResponse<AttendanceRegularizationResponseDto>> GetRequestsAsync(AttendanceRegularizationFilterDto filter, CancellationToken cancellationToken = default);

    Task<AttendanceRegularizationResponseDto> ApproveAsync(Guid regularizationId, ApproveAttendanceRegularizationDto dto, CancellationToken cancellationToken = default);

    Task<AttendanceRegularizationResponseDto> RejectAsync(Guid regularizationId, RejectAttendanceRegularizationDto dto, CancellationToken cancellationToken = default);
}
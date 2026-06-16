using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.EmployeeEducation;

namespace HRMS.API.Interfaces;

public interface IEmployeeEducationService
{
    Task<EmployeeEducationResponseDto> AddEducationAsync(AddEmployeeEducationDto dto, CancellationToken cancellationToken = default);

    Task<PagedResponse<EmployeeEducationResponseDto>> GetEducationsAsync(EmployeeEducationFilterDto filter, CancellationToken cancellationToken = default);

    Task<EmployeeEducationResponseDto> UpdateEducationAsync(Guid educationId, UpdateEmployeeEducationDto dto, CancellationToken cancellationToken = default);

    Task DeleteEducationAsync(Guid educationId, CancellationToken cancellationToken = default);

    Task<byte[]> ExportEducationsAsync(EmployeeEducationFilterDto filter, CancellationToken cancellationToken = default);

    Task<EmployeeEducationImportResultDto> ImportEducationsAsync(IFormFile file, CancellationToken cancellationToken = default);
}
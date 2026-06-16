using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.EmployeeExperience;

namespace HRMS.API.Interfaces;

public interface IEmployeeExperienceService
{
    Task<EmployeeExperienceResponseDto> AddExperienceAsync(AddEmployeeExperienceDto dto, CancellationToken cancellationToken = default);

    Task<PagedResponse<EmployeeExperienceResponseDto>> GetExperiencesAsync(EmployeeExperienceFilterDto filter, CancellationToken cancellationToken = default);

    Task<EmployeeExperienceResponseDto> UpdateExperienceAsync(Guid experienceId, UpdateEmployeeExperienceDto dto, CancellationToken cancellationToken = default);

    Task DeleteExperienceAsync(Guid experienceId, CancellationToken cancellationToken = default);

    Task<byte[]> ExportExperiencesAsync(EmployeeExperienceFilterDto filter, CancellationToken cancellationToken = default);

    Task<EmployeeExperienceImportResultDto> ImportExperiencesAsync(IFormFile file, CancellationToken cancellationToken = default);
}
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.EmployeeEmergencyContact;

namespace HRMS.API.Interfaces;

public interface IEmployeeEmergencyContactService
{
    Task<EmployeeEmergencyContactResponseDto> AddContactAsync(AddEmployeeEmergencyContactDto dto, CancellationToken cancellationToken = default);

    Task<PagedResponse<EmployeeEmergencyContactResponseDto>> GetContactsAsync(EmployeeEmergencyContactFilterDto filter, CancellationToken cancellationToken = default);

    Task<EmployeeEmergencyContactResponseDto> UpdateContactAsync(Guid contactId, UpdateEmployeeEmergencyContactDto dto, CancellationToken cancellationToken = default);

    Task DeleteContactAsync(Guid contactId, CancellationToken cancellationToken = default);

    Task<byte[]> ExportContactsAsync(EmployeeEmergencyContactFilterDto filter, CancellationToken cancellationToken = default);

    Task<EmployeeEmergencyContactImportResultDto> ImportContactsAsync(IFormFile file, CancellationToken cancellationToken = default);
}
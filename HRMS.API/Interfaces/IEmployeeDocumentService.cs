using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.EmployeeDocument;

namespace HRMS.API.Interfaces;

public interface IEmployeeDocumentService
{
    Task<EmployeeDocumentResponseDto> UploadDocumentAsync(AddEmployeeDocumentDto dto, CancellationToken cancellationToken = default);

    Task<PagedResponse<EmployeeDocumentResponseDto>> GetDocumentsAsync(EmployeeDocumentFilterDto filter, CancellationToken cancellationToken = default);

    Task VerifyDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task DeleteDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<(byte[] FileBytes, string FileName, string ContentType)> DownloadDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<byte[]> ExportDocumentsAsync(EmployeeDocumentFilterDto filter, CancellationToken cancellationToken = default);
}
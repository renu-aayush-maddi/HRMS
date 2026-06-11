using HRMS.API.Models.DTOs.EmployeeDocument;

namespace HRMS.API.Interfaces;

public interface IEmployeeDocumentService
{
    Task UploadDocument(
        AddEmployeeDocumentDto dto);

    List<EmployeeDocumentResponseDto>
        GetEmployeeDocuments(
            Guid employeeId);

    void VerifyDocument(
        Guid documentId,
        Guid verifiedBy);

    void DeleteDocument(
        Guid documentId);
}
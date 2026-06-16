using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IEmployeeDocumentRepository
{
    Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<EmployeeDocument?> GetDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);

    IQueryable<EmployeeDocument> GetDocuments();

    Task<bool> DocumentExistsAsync(Guid employeeId, string documentType, CancellationToken cancellationToken = default);

    Task AddDocumentAsync(EmployeeDocument document, CancellationToken cancellationToken = default);

    void UpdateDocument(EmployeeDocument document);

    void DeleteDocument(EmployeeDocument document);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
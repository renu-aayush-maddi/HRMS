using HRMS.API.Models.Entities;

namespace HRMS.API.Interfaces;

public interface IEmployeeDocumentRepository
{
    Employee? GetEmployee(Guid employeeId);

    EmployeeDocument? GetDocument(Guid documentId);

    List<EmployeeDocument>
        GetEmployeeDocuments(Guid employeeId);

    void AddDocument(
        EmployeeDocument document);

    void UpdateDocument(
        EmployeeDocument document);

    void DeleteDocument(
        EmployeeDocument document);

    void SaveChanges();
}
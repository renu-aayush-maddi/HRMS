using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.EmployeeDocument;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

public class EmployeeDocumentService
    : IEmployeeDocumentService
{
    private readonly
        IEmployeeDocumentRepository repository;

    public EmployeeDocumentService(
        IEmployeeDocumentRepository repository)
    {
        this.repository = repository;
    }

    public async Task UploadDocument(
    AddEmployeeDocumentDto dto)
{
    var employee =
        repository.GetEmployee(
            dto.EmployeeId);

    if (employee == null)
    {
        throw new Exception(
            "Employee not found");
    }

    string uploadsFolder =
        Path.Combine(
            Directory.GetCurrentDirectory(),
            "Uploads");

    Directory.CreateDirectory(
        uploadsFolder);

    string fileName =
        $"{Guid.NewGuid()}" +
        Path.GetExtension(
            dto.File.FileName);

    string filePath =
        Path.Combine(
            uploadsFolder,
            fileName);

    using FileStream stream =
        new(
            filePath,
            FileMode.Create);

    await dto.File.CopyToAsync(
        stream);

    EmployeeDocument document =
        new()
        {
            Id = Guid.NewGuid(),

            EmployeeId =
                dto.EmployeeId,

            DocumentName =
                dto.File.FileName,

            DocumentType =
                dto.DocumentType,

            FileUrl =
                $"/Uploads/{fileName}"
        };

    repository.AddDocument(
        document);

    repository.SaveChanges();
}
    public List<EmployeeDocumentResponseDto>
        GetEmployeeDocuments(
            Guid employeeId)
    {
        return repository
            .GetEmployeeDocuments(
                employeeId)
            .Select(d =>
                new EmployeeDocumentResponseDto
                {
                    Id = d.Id,

                    DocumentName =
                        d.DocumentName,

                    DocumentType =
                        d.DocumentType ?? "",

                    FileUrl =
                        d.FileUrl,

                    IsVerified =
                        d.IsVerified,

                    UploadedAt =
                        d.UploadedAt
                })
            .ToList();
    }

    public void VerifyDocument(
        Guid documentId,
        Guid verifiedBy)
    {
        var document =
            repository.GetDocument(
                documentId);

        if(document == null)
        {
            throw new Exception(
                "Document not found");
        }

        document.IsVerified = true;

        document.VerifiedBy =
            verifiedBy;

        document.VerifiedAt =
            DateTime.Now;

        repository.UpdateDocument(
            document);

        repository.SaveChanges();
    }

    public void DeleteDocument(
        Guid documentId)
    {
        var document =
            repository.GetDocument(
                documentId);

        if(document == null)
        {
            throw new Exception(
                "Document not found");
        }

        repository.DeleteDocument(
            document);

        repository.SaveChanges();
    }
}
using ClosedXML.Excel;
using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.EmployeeDocument;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Services;

public class EmployeeDocumentService : IEmployeeDocumentService
{
    private readonly IEmployeeDocumentRepository repository;
    private readonly IEmployeeAccessResolver accessResolver;
    private readonly IAuditLogService auditLogService;
    private readonly ILogger<EmployeeDocumentService> logger;

    private readonly IUserContextService userContextService;

    public EmployeeDocumentService(
        IEmployeeDocumentRepository repository,
        IEmployeeAccessResolver accessResolver,
        IAuditLogService auditLogService,
        IUserContextService userContextService,
        ILogger<EmployeeDocumentService> logger)
    {
        this.repository = repository;
        this.accessResolver = accessResolver;
        this.auditLogService = auditLogService;
        this.userContextService = userContextService;
        this.logger = logger;
    }

    public async Task<EmployeeDocumentResponseDto> UploadDocumentAsync(AddEmployeeDocumentDto dto, CancellationToken cancellationToken = default)
    {
        var employeeId = await accessResolver.ResolveEmployeeIdAsync(dto.EmployeeId, cancellationToken);
        var employee = await repository.GetEmployeeAsync(employeeId, cancellationToken);

        if (employee == null) throw new NotFoundException("Employee not found.");

        var exists = await repository.DocumentExistsAsync(employeeId, dto.DocumentType, cancellationToken);
        if (exists) throw new BusinessException($"Document of type '{dto.DocumentType}' already exists.");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.File.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream, cancellationToken);
        }

        var document = new EmployeeDocument
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            DocumentName = dto.File.FileName,
            DocumentType = dto.DocumentType,
            FileUrl = $"/Uploads/{fileName}",
            UploadedAt = DateTime.Now,
            IsVerified = false
        };

        await repository.AddDocumentAsync(document, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Create", nameof(EmployeeDocument), document.Id, $"Document uploaded: {dto.DocumentType}", cancellationToken);
        logger.LogInformation("Document {DocumentId} uploaded", document.Id);

        return MapToResponse(document);
    }

    public async Task<PagedResponse<EmployeeDocumentResponseDto>> GetDocumentsAsync(EmployeeDocumentFilterDto filter, CancellationToken cancellationToken = default)
    {
        var employeeId = await accessResolver.ResolveEmployeeIdAsync(filter.EmployeeId, cancellationToken);
        await accessResolver.ValidateEmployeeOwnershipAsync(employeeId, cancellationToken);

        var query = repository.GetDocuments().Where(x => x.EmployeeId == employeeId);

        if (!string.IsNullOrWhiteSpace(filter.DocumentType))
            query = query.Where(x => x.DocumentType != null && x.DocumentType.Contains(filter.DocumentType));

        if (filter.IsVerified.HasValue)
            query = query.Where(x => x.IsVerified == filter.IsVerified);

        query = ApplySorting(query, filter);

        var totalRecords = await query.CountAsync(cancellationToken);
        var documents = await query.Skip((filter.PageNumber - 1) * filter.PageSize)
                                   .Take(filter.PageSize)
                                   .ToListAsync(cancellationToken);

        return new PagedResponse<EmployeeDocumentResponseDto>
        {
            Data = documents.Select(MapToResponse).ToList(),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize)
        };
    }

    public async Task VerifyDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var document = await repository.GetDocumentAsync(documentId, cancellationToken);

        if (document == null)
        {
            throw new NotFoundException("Document not found.");
        }

        if (document.IsVerified == true)
        {
            throw new BusinessException("Document already verified.");
        }

        document.IsVerified = true;
        document.VerifiedAt = DateTime.Now;
        document.VerifiedBy = userContextService.GetUserId();

        repository.UpdateDocument(document);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Verify",
            nameof(EmployeeDocument),
            document.Id,
            "Document verified",
            cancellationToken);
    }

    public async Task DeleteDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var document = await repository.GetDocumentAsync(documentId, cancellationToken);
        if (document == null) throw new NotFoundException("Document not found.");

        await accessResolver.ValidateEmployeeOwnershipAsync(document.EmployeeId!.Value, cancellationToken);

        var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), document.FileUrl.TrimStart('/'));
        if (File.Exists(physicalPath)) File.Delete(physicalPath);

        repository.DeleteDocument(document);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync("Delete", nameof(EmployeeDocument), document.Id, "Document deleted", cancellationToken);
    }

    public async Task<(byte[] FileBytes, string FileName, string ContentType)> DownloadDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var document = await repository.GetDocumentAsync(documentId, cancellationToken);
        if (document == null) throw new NotFoundException("Document not found.");

        await accessResolver.ValidateEmployeeOwnershipAsync(document.EmployeeId!.Value, cancellationToken);

        var path = Path.Combine(Directory.GetCurrentDirectory(), document.FileUrl.TrimStart('/'));
        if (!File.Exists(path)) throw new NotFoundException("Physical file not found.");

        var bytes = await File.ReadAllBytesAsync(path, cancellationToken);

        await auditLogService.LogAsync("Download", nameof(EmployeeDocument), document.Id, "Document downloaded", cancellationToken);

        return (bytes, document.DocumentName, "application/octet-stream");
    }

    public async Task<byte[]> ExportDocumentsAsync(EmployeeDocumentFilterDto filter, CancellationToken cancellationToken = default)
    {
        var employeeId = await accessResolver.ResolveEmployeeIdAsync(filter.EmployeeId, cancellationToken);
        var documents = await repository.GetDocuments().Where(x => x.EmployeeId == employeeId).ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Documents");

        worksheet.Cell(1, 1).Value = "Document Name";
        worksheet.Cell(1, 2).Value = "Document Type";
        worksheet.Cell(1, 3).Value = "Verified";
        worksheet.Cell(1, 4).Value = "Uploaded At";

        int row = 2;
        foreach (var document in documents)
        {
            worksheet.Cell(row, 1).Value = document.DocumentName;
            worksheet.Cell(row, 2).Value = document.DocumentType;
            worksheet.Cell(row, 3).Value = document.IsVerified?.ToString() ?? "N/A";
            worksheet.Cell(row, 4).Value = document.UploadedAt;
            row++;
        }

        worksheet.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static EmployeeDocumentResponseDto MapToResponse(EmployeeDocument document) => new()
    {
        Id = document.Id,
        EmployeeId = document.EmployeeId ?? Guid.Empty,
        DocumentName = document.DocumentName,
        DocumentType = document.DocumentType ?? string.Empty,
        FileUrl = document.FileUrl,
        IsVerified = document.IsVerified,
        UploadedAt = document.UploadedAt
    };

    private static IQueryable<EmployeeDocument> ApplySorting(IQueryable<EmployeeDocument> query, EmployeeDocumentFilterDto filter)
    {
        return filter.SortBy?.ToLower() switch
        {
            "documenttype" => filter.Descending ? query.OrderByDescending(x => x.DocumentType) : query.OrderBy(x => x.DocumentType),
            "uploadedat" => filter.Descending ? query.OrderByDescending(x => x.UploadedAt) : query.OrderBy(x => x.UploadedAt),
            _ => query.OrderByDescending(x => x.UploadedAt)
        };
    }
}
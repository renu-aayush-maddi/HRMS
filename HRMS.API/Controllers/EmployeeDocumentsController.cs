using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.EmployeeDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/employee-documents")]
[ApiController]
[Authorize]
public class EmployeeDocumentsController : ControllerBase
{
    private readonly IEmployeeDocumentService service;

    public EmployeeDocumentsController(IEmployeeDocumentService service)
    {
        this.service = service;
    }

    [HttpPost]
    public async Task<IActionResult> UploadDocument([FromForm] AddEmployeeDocumentDto dto, CancellationToken cancellationToken)
    {
        var result = await service.UploadDocumentAsync(dto, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetDocuments([FromQuery] EmployeeDocumentFilterDto filter, CancellationToken cancellationToken)
    {
        var result = await service.GetDocumentsAsync(filter, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{documentId:guid}/verify")]
    public async Task<IActionResult> VerifyDocument(Guid documentId, CancellationToken cancellationToken)
    {
        await service.VerifyDocumentAsync(documentId, cancellationToken);
        return Ok(new ApiResponse { Message = "Document verified successfully." });
    }

    [HttpGet("{documentId:guid}/download")]
    public async Task<IActionResult> DownloadDocument(Guid documentId, CancellationToken cancellationToken)
    {
        var result = await service.DownloadDocumentAsync(documentId, cancellationToken);
        return File(result.FileBytes, result.ContentType, result.FileName);
    }

    [HttpDelete("{documentId:guid}")]
    public async Task<IActionResult> DeleteDocument(Guid documentId, CancellationToken cancellationToken)
    {
        await service.DeleteDocumentAsync(documentId, cancellationToken);
        return Ok(new ApiResponse { Message = "Document deleted successfully." });
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpGet("export")]
    public async Task<IActionResult> ExportDocuments([FromQuery] EmployeeDocumentFilterDto filter, CancellationToken cancellationToken)
    {
        var file = await service.ExportDocumentsAsync(filter, cancellationToken);
        return File(
            file, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
            $"employee-documents-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }
}
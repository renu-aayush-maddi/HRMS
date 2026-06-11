using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.EmployeeDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.API.Controllers;

[Route("api/employee-documents")]
[ApiController]
[Authorize]
public class EmployeeDocumentsController
    : ControllerBase
{
    private readonly
        IEmployeeDocumentService service;

    public EmployeeDocumentsController(
        IEmployeeDocumentService service)
    {
        this.service = service;
    }

    [Authorize(Roles = "Admin,HR,Employee,Manager")]
    [HttpPost]
    public async Task<IActionResult> UploadDocument(
        [FromForm] AddEmployeeDocumentDto dto)
    {
        await service.UploadDocument(dto);

        return Ok(
            "Document Uploaded Successfully");
    }

    [Authorize(Roles = "Admin,HR,Manager")]
    [HttpGet("{employeeId}")]
    public IActionResult GetDocuments(
        Guid employeeId)
    {
        return Ok(
            service.GetEmployeeDocuments(
                employeeId));
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpPut("{documentId}/verify")]
    public IActionResult VerifyDocument(
        Guid documentId)
    {
        var userId =
            Guid.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier)!
                .Value);

        service.VerifyDocument(
            documentId,
            userId);

        return Ok(
            "Document Verified");
    }

    [Authorize(Roles = "Admin,HR")]
    [HttpDelete("{documentId}")]
    public IActionResult DeleteDocument(
        Guid documentId)
    {
        service.DeleteDocument(
            documentId);

        return Ok(
            "Document Deleted");
    }
}
namespace HRMS.API.Models.DTOs.EmployeeDocument;

public class EmployeeDocumentResponseDto
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public string DocumentName { get; set; } = string.Empty;

    public string DocumentType { get; set; } = string.Empty;

    public string FileUrl { get; set; } = string.Empty;

    public bool? IsVerified { get; set; }

    public DateTime? UploadedAt { get; set; }
}
namespace HRMS.API.Models.DTOs.EmployeeDocument;

public class AddEmployeeDocumentDto
{
    public Guid? EmployeeId { get; set; }

    public string DocumentType { get; set; } = string.Empty;

    public IFormFile File { get; set; } = null!;
}
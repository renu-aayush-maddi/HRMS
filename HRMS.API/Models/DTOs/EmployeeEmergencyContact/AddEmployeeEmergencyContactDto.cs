namespace HRMS.API.Models.DTOs.EmployeeEmergencyContact;

public class AddEmployeeEmergencyContactDto
{
    public Guid? EmployeeId { get; set; }

    public string ContactName { get; set; } = string.Empty;

    public string Relationship { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}
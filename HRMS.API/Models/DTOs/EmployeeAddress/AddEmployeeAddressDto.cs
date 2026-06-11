namespace HRMS.API.Models.DTOs.EmployeeAddress;

public class AddEmployeeAddressDto
{
    public Guid EmployeeId { get; set; }

    public string AddressLine1 { get; set; }
        = string.Empty;

    public string? AddressLine2 { get; set; }

    public string City { get; set; }
        = string.Empty;

    public string State { get; set; }
        = string.Empty;

    public string Country { get; set; }
        = string.Empty;

    public string PostalCode { get; set; }
        = string.Empty;

    public string AddressType { get; set; }
        = string.Empty;
}
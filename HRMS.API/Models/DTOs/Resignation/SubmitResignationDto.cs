namespace HRMS.API.Models.DTOs.Resignation;

public class SubmitResignationDto
{
    public Guid EmployeeId { get; set; }

    public DateOnly LastWorkingDate { get; set; }

    public string Reason { get; set; }
        = string.Empty;
}
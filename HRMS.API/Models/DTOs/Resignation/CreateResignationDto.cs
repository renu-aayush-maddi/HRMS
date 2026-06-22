namespace HRMS.API.Models.DTOs.Resignation;

public class CreateResignationDto
{
    public DateOnly LastWorkingDate { get; set; }

    public string Reason { get; set; } = string.Empty;
}
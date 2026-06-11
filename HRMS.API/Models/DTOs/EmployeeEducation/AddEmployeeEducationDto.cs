namespace HRMS.API.Models.DTOs.EmployeeEducation;

public class AddEmployeeEducationDto
{
    public Guid EmployeeId { get; set; }

    public string Degree { get; set; }
        = string.Empty;

    public string Specialization { get; set; }
        = string.Empty;

    public string InstitutionName { get; set; }
        = string.Empty;

    public int GraduationYear { get; set; }

    public decimal? Percentage { get; set; }
}
namespace HRMS.API.Models.DTOs.Holiday;

public class HolidayResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateOnly HolidayDate { get; set; }

    public string? Description { get; set; }

    public bool? IsOptional { get; set; }
}
namespace HRMS.API.Models.DTOs.PerformanceCycle;

public class PerformanceCycleResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }
        = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string Status { get; set; }
        = string.Empty;
}
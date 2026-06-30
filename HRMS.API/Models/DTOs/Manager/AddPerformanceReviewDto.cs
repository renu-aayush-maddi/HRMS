namespace HRMS.API.Models.DTOs.Manager;

public class AddPerformanceReviewDto
{
    public Guid EmployeeId { get; set; }

    public Guid PerformanceCycleId { get; set; }

    public decimal Rating { get; set; }

    public string Comments { get; set; }
        = string.Empty;
}
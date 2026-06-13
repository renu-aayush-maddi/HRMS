namespace HRMS.API.Models.DTOs.Dashboard;

public class HrDashboardDto
{
    public int TotalEmployees { get; set; }

    public int TotalReviews { get; set; }

    public decimal AverageRating { get; set; }

    public decimal ReviewCompletionPercentage { get; set; }

    public List<EmployeePerformanceDto> TopPerformers { get; set; }
        = [];

    public List<EmployeePerformanceDto> LowestPerformers { get; set; }
        = [];

    public List<DepartmentRatingDto> DepartmentRatings { get; set; }
        = [];
}
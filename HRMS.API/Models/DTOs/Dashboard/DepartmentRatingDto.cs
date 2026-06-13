namespace HRMS.API.Models.DTOs.Dashboard;

public class DepartmentRatingDto
{
    public string DepartmentName { get; set; }
        = string.Empty;

    public decimal AverageRating { get; set; }
}
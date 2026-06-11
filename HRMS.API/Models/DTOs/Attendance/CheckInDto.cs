using HRMS.API.Validations;

namespace HRMS.API.Models.DTOs.Attendance;

public class CheckInDto
{
    [ValidGuid(ErrorMessage ="EmployeeId is required")]
    public Guid EmployeeId { get; set; }
}
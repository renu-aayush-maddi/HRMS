using HRMS.API.Validations;
using System.ComponentModel.DataAnnotations;

public class CheckOutDto
{
    [ValidGuid(ErrorMessage ="EmployeeId is required")]
    public Guid EmployeeId { get; set; }
}
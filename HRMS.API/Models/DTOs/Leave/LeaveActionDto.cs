using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Models.DTOs.Leave;

public class LeaveActionDto
{
    [StringLength(500)]
    public string? ManagerComments { get; set; }
}
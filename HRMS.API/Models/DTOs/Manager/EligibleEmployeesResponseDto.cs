using System.Collections.Generic;

namespace HRMS.API.Models.DTOs.Manager;

public class EligibleEmployeesResponseDto
{
    public List<EligibleEmployeeDto> Employees { get; set; } = new();
    public int TotalCount { get; set; }
}

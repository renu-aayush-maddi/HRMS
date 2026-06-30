using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HRMS.API.Models.DTOs.Employee;

namespace HRMS.API.Interfaces;

public interface IHierarchyService
{
    Task<List<EmployeeNodeDto>> GetEntireTreeAsync(CancellationToken cancellationToken = default);
    
    Task<List<EmployeeNodeDto>> GetSubtreeAsync(Guid employeeId, CancellationToken cancellationToken = default);
    
    Task<List<EmployeeNodeDto>> GetReportingChainAsync(Guid employeeId, CancellationToken cancellationToken = default);
    
    Task<List<EmployeeNodeDto>> GetDirectReportsAsync(Guid employeeId, CancellationToken cancellationToken = default);
    
    Task<ManagerInfoDto?> GetManagerInfoAsync(Guid employeeId, CancellationToken cancellationToken = default);
    
    Task<List<EmployeeNodeDto>> SearchEmployeesAsync(string query, CancellationToken cancellationToken = default);
}

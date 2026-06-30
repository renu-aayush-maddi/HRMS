using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Employee;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Services;

public class HierarchyService : IHierarchyService
{
    private readonly IEmployeeRepository employeeRepository;

    public HierarchyService(IEmployeeRepository employeeRepository)
    {
        this.employeeRepository = employeeRepository;
    }

    public async Task<List<EmployeeNodeDto>> GetEntireTreeAsync(CancellationToken cancellationToken = default)
    {
        var allEmployees = await employeeRepository.GetActiveEmployeesForHierarchyAsync(cancellationToken);
        return BuildTree(allEmployees);
    }

    public async Task<List<EmployeeNodeDto>> GetSubtreeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var allEmployees = await employeeRepository.GetActiveEmployeesForHierarchyAsync(cancellationToken);
        var entireTree = BuildTree(allEmployees);
        
        var node = FindNodeInTree(entireTree, employeeId);
        if (node != null)
        {
            return new List<EmployeeNodeDto> { node };
        }
        
        return new List<EmployeeNodeDto>();
    }

    public async Task<List<EmployeeNodeDto>> GetReportingChainAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var chain = new List<EmployeeNodeDto>();
        var allEmployees = await employeeRepository.GetActiveEmployeesForHierarchyAsync(cancellationToken);
        var allMap = allEmployees.ToDictionary(e => e.Id);
        
        var visited = new HashSet<Guid>();
        var currentId = employeeId;
        
        while (allMap.TryGetValue(currentId, out var employee))
        {
            if (!visited.Add(currentId))
            {
                // Break cycle
                break;
            }
            
            chain.Add(new EmployeeNodeDto
            {
                Id = employee.Id,
                ManagerId = employee.ManagerId,
                EmployeeCode = employee.EmployeeCode,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Designation = employee.Designation,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name,
                EmploymentStatus = employee.EmploymentStatus,
                ProfilePhotoUrl = employee.ProfilePhotoUrl
            });
            
            if (!employee.ManagerId.HasValue)
            {
                break;
            }
            
            currentId = employee.ManagerId.Value;
        }
        
        chain.Reverse();
        return chain;
    }

    public async Task<List<EmployeeNodeDto>> GetDirectReportsAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var employees = await employeeRepository.GetDirectReportsAsync(employeeId, cancellationToken);
            
        return employees.Select(e => new EmployeeNodeDto
        {
            Id = e.Id,
            ManagerId = e.ManagerId,
            EmployeeCode = e.EmployeeCode,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            Designation = e.Designation,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department?.Name,
            EmploymentStatus = e.EmploymentStatus,
            ProfilePhotoUrl = e.ProfilePhotoUrl
        }).ToList();
    }

    public async Task<ManagerInfoDto?> GetManagerInfoAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetEmployeeByIdAsync(employeeId, cancellationToken);
            
        if (employee == null || !employee.ManagerId.HasValue) return null;
        
        var manager = await employeeRepository.GetEmployeeByIdAsync(employee.ManagerId.Value, cancellationToken);
            
        if (manager == null) return null;
        
        return new ManagerInfoDto
        {
            Id = manager.Id,
            EmployeeCode = manager.EmployeeCode,
            Name = $"{manager.FirstName} {manager.LastName}",
            Designation = manager.Designation,
            Department = manager.Department?.Name,
            ProfilePhotoUrl = manager.ProfilePhotoUrl,
            EmploymentStatus = manager.EmploymentStatus
        };
    }

    public async Task<List<EmployeeNodeDto>> SearchEmployeesAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query)) return new List<EmployeeNodeDto>();
        
        var employees = await employeeRepository.SearchActiveEmployeesAsync(query, cancellationToken);
            
        return employees.Select(e => new EmployeeNodeDto
        {
            Id = e.Id,
            ManagerId = e.ManagerId,
            EmployeeCode = e.EmployeeCode,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            Designation = e.Designation,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department?.Name,
            EmploymentStatus = e.EmploymentStatus,
            ProfilePhotoUrl = e.ProfilePhotoUrl
        }).ToList();
    }

    private List<EmployeeNodeDto> BuildTree(List<Employee> allEmployees)
    {
        var activeEmployees = allEmployees.Where(e => e.EmploymentStatus == "Active").ToList();
        var allMap = allEmployees.ToDictionary(e => e.Id);
        
        var nodes = activeEmployees.Select(e => new EmployeeNodeDto
        {
            Id = e.Id,
            ManagerId = e.ManagerId,
            EmployeeCode = e.EmployeeCode,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            Designation = e.Designation,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department?.Name,
            EmploymentStatus = e.EmploymentStatus,
            ProfilePhotoUrl = e.ProfilePhotoUrl
        }).ToDictionary(n => n.Id);
        
        var managerMap = activeEmployees.ToDictionary(e => e.Id, e => e.ManagerId);
        
        foreach (var node in nodes.Values)
        {
            if (node.ManagerId.HasValue)
            {
                var mId = node.ManagerId.Value;
                
                if (IsCircular(node.Id, mId, managerMap))
                {
                    node.Warning = "Circular manager relationship detected";
                    node.ManagerId = null;
                    continue;
                }
                
                if (nodes.TryGetValue(mId, out var managerNode))
                {
                    managerNode.Children.Add(node);
                    managerNode.DirectReportsCount++;
                }
                else
                {
                    if (allMap.TryGetValue(mId, out var managerEntity))
                    {
                        node.Warning = $"Reports to Inactive/Resigned Manager ({managerEntity.FirstName} {managerEntity.LastName})";
                    }
                    else
                    {
                        node.Warning = "Reports to a missing/deleted Manager";
                    }
                    node.ManagerId = null;
                }
            }
        }
        
        return nodes.Values.Where(n => n.ManagerId == null).ToList();
    }

    private bool IsCircular(Guid startId, Guid? managerId, Dictionary<Guid, Guid?> managerMap)
    {
        var visited = new HashSet<Guid> { startId };
        var current = managerId;
        while (current.HasValue)
        {
            if (!visited.Add(current.Value))
            {
                return true;
            }
            managerMap.TryGetValue(current.Value, out current);
        }
        return false;
    }

    private EmployeeNodeDto? FindNodeInTree(List<EmployeeNodeDto> nodes, Guid targetId)
    {
        foreach (var node in nodes)
        {
            if (node.Id == targetId) return node;
            var found = FindNodeInTree(node.Children, targetId);
            if (found != null) return found;
        }
        return null;
    }
}

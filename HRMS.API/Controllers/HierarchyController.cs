using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class HierarchyController : ControllerBase
{
    private readonly IHierarchyService hierarchyService;
    private readonly IUserContextService userContextService;

    public HierarchyController(IHierarchyService hierarchyService, IUserContextService userContextService)
    {
        this.hierarchyService = hierarchyService;
        this.userContextService = userContextService;
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetTree([FromQuery] Guid? rootId, [FromQuery] string? scope, CancellationToken cancellationToken)
    {
        var role = userContextService.GetRole();
        var userEmployeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
        
        if (userEmployeeId == null)
        {
            return BadRequest("Logged-in user is not associated with an employee profile.");
        }

        // HR & Admin can see any tree/subtree
        if (userContextService.IsAdminOrHr())
        {
            if (rootId.HasValue)
            {
                var subtree = await hierarchyService.GetSubtreeAsync(rootId.Value, cancellationToken);
                return Ok(subtree);
            }
            var entireTree = await hierarchyService.GetEntireTreeAsync(cancellationToken);
            return Ok(entireTree);
        }

        // Managers can see the entire organization chart, but if they explicitly ask for "scope=team" or their team subtree, we return that.
        if (role == "Manager")
        {
            if (scope == "team")
            {
                var teamTree = await hierarchyService.GetSubtreeAsync(userEmployeeId.Value, cancellationToken);
                return Ok(teamTree);
            }

            if (rootId.HasValue)
            {
                // Verify rootId is within manager's subtree
                var managerSubtree = await hierarchyService.GetSubtreeAsync(userEmployeeId.Value, cancellationToken);
                if (!IsNodeInTree(managerSubtree, rootId.Value))
                {
                    return Forbid();
                }
                var subtree = await hierarchyService.GetSubtreeAsync(rootId.Value, cancellationToken);
                return Ok(subtree);
            }

            var entireTree = await hierarchyService.GetEntireTreeAsync(cancellationToken);
            return Ok(entireTree);
        }

        // Regular Employees can see the entire organization hierarchy, but cannot request subtrees of other employees directly unless it is themselves.
        if (rootId.HasValue && rootId.Value != userEmployeeId.Value)
        {
            return Forbid();
        }

        var generalTree = await hierarchyService.GetEntireTreeAsync(cancellationToken);
        return Ok(generalTree);
    }

    [HttpGet("reporting-chain/{employeeId:guid}")]
    public async Task<IActionResult> GetReportingChain(Guid employeeId, CancellationToken cancellationToken)
    {
        var chain = await hierarchyService.GetReportingChainAsync(employeeId, cancellationToken);
        return Ok(chain);
    }

    [HttpGet("direct-reports/{employeeId:guid}")]
    public async Task<IActionResult> GetDirectReports(Guid employeeId, CancellationToken cancellationToken)
    {
        var reports = await hierarchyService.GetDirectReportsAsync(employeeId, cancellationToken);
        return Ok(reports);
    }

    [HttpGet("manager/{employeeId:guid}")]
    public async Task<IActionResult> GetManagerInfo(Guid employeeId, CancellationToken cancellationToken)
    {
        var managerInfo = await hierarchyService.GetManagerInfoAsync(employeeId, cancellationToken);
        if (managerInfo == null)
        {
            return NotFound("No manager found or manager is inactive.");
        }
        return Ok(managerInfo);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query, CancellationToken cancellationToken)
    {
        var results = await hierarchyService.SearchEmployeesAsync(query, cancellationToken);
        return Ok(results);
    }

    private bool IsNodeInTree(List<EmployeeNodeDto> nodes, Guid targetId)
    {
        foreach (var node in nodes)
        {
            if (node.Id == targetId) return true;
            if (IsNodeInTree(node.Children, targetId)) return true;
        }
        return false;
    }
}

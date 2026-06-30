using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Manager;
using HRMS.API.Models.Entities;
using HRMS.API.Exceptions;
using System.Linq;

namespace HRMS.API.Services;

public class ManagerService : IManagerService
{
    private readonly IManagerRepository repository;
    private readonly IAuditLogService auditLogService;
    private readonly INotificationService notificationService;

    public ManagerService(
        IManagerRepository repository,
        IAuditLogService auditLogService,
        INotificationService notificationService)
    {
        this.repository = repository;
        this.auditLogService = auditLogService;
        this.notificationService = notificationService;
    }

    public ManagerDashboardDto GetDashboard(Guid managerUserId)
    {
        var manager = repository.GetEmployeeByUserId(managerUserId);

        if (manager == null)
        {
            throw new Exception("Manager not found");
        }

        return repository.GetDashboard(manager.Id);
    }

    public List<TeamMemberDto> GetTeamMembers(Guid managerUserId)
    {
        var manager = repository.GetEmployeeByUserId(managerUserId);

        if (manager == null)
        {
            throw new Exception("Manager not found");
        }

        return repository.GetTeamMembers(manager.Id)
            .Select(e => new TeamMemberDto
            {
                Id = e.Id,
                EmployeeCode = e.EmployeeCode,
                FullName = e.FirstName + " " + e.LastName,
                Email = e.Email,
                Designation = e.Designation ?? "",
                Department = e.Department?.Name ?? "",
                EmploymentStatus = e.EmploymentStatus ?? ""
            })
            .ToList();
    }

    public TeamMemberDto GetTeamMember(Guid managerUserId, Guid employeeId)
    {
        var manager = repository.GetEmployeeByUserId(managerUserId);

        if (manager == null)
        {
            throw new Exception("Manager not found");
        }

        var employee = repository.GetTeamMember(manager.Id, employeeId);

        if (employee == null)
        {
            throw new Exception("Employee not found");
        }

        return new TeamMemberDto
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            FullName = employee.FirstName + " " + employee.LastName,
            Email = employee.Email,
            Designation = employee.Designation ?? "",
            Department = employee.Department?.Name ?? "",
            EmploymentStatus = employee.EmploymentStatus ?? ""
        };
    }

    public List<TeamAttendanceDto> GetTeamAttendance(Guid managerUserId)
    {
        var manager = repository.GetEmployeeByUserId(managerUserId);

        if (manager == null)
        {
            throw new Exception("Manager not found");
        }

        return repository.GetTeamAttendance(manager.Id)
            .Select(a => new TeamAttendanceDto
            {
                EmployeeName = a.Employee!.FirstName + " " + a.Employee.LastName,
                AttendanceDate = a.AttendanceDate,
                CheckIn = a.CheckIn,
                CheckOut = a.CheckOut,
                Status = a.Status
            })
            .ToList();
    }

    public List<LateEmployeeDto> GetLateEmployees(Guid managerUserId)
    {
        var manager = repository.GetEmployeeByUserId(managerUserId);

        if (manager == null)
        {
            throw new Exception("Manager not found");
        }

        return repository.GetLateEmployees(manager.Id)
            .Select(a => new LateEmployeeDto
            {
                EmployeeName = a.Employee!.FirstName + " " + a.Employee.LastName,
                CheckIn = a.CheckIn
            })
            .ToList();
    }

    public List<PendingLeaveDto> GetPendingLeaveRequests(Guid managerUserId)
    {
        var manager = repository.GetEmployeeByUserId(managerUserId);

        if (manager == null)
        {
            throw new Exception("Manager not found");
        }

        return repository.GetPendingLeaveRequests(manager.Id)
            .Select(l => new PendingLeaveDto
            {
                LeaveId = l.Id,
                EmployeeName = l.Employee!.FirstName + " " + l.Employee.LastName,
                FromDate = l.FromDate,
                ToDate = l.ToDate,
                Reason = l.Reason,
                Status = l.Status
            })
            .ToList();
    }

    public void AddPerformanceReview(Guid managerUserId, AddPerformanceReviewDto dto)
    {
        var manager = repository.GetEmployeeByUserId(managerUserId);

        if (manager == null)
        {
            throw new Exception("Manager not found");
        }

        var employee = repository.GetTeamMember(manager.Id, dto.EmployeeId);

        if (employee == null)
        {
            throw new Exception("Employee does not belong to your team");
        }

        if (dto.PerformanceCycleId == Guid.Empty)
        {
            throw new Exception("Performance cycle is required");
        }

        var cycle = repository.GetPerformanceCycle(dto.PerformanceCycleId);
        if (cycle == null)
        {
            throw new Exception("Performance cycle not found");
        }

        if (!string.Equals(cycle.Status, "Active", StringComparison.OrdinalIgnoreCase))
        {
            throw new Exception("Performance cycle is not active");
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        if (today < cycle.StartDate || today > cycle.EndDate)
        {
            throw new Exception("Reviews can only be submitted during the performance cycle dates");
        }

        if (dto.Rating < 1 || dto.Rating > 5)
        {
            throw new Exception("Rating must be between 1 and 5");
        }

        PerformanceReview review = new()
        {
            Id = Guid.NewGuid(),
            EmployeeId = dto.EmployeeId,
            ReviewerId = manager.Id,
            PerformanceCycleId = dto.PerformanceCycleId,
            Rating = dto.Rating,
            Comments = dto.Comments,
            ReviewDate = today
        };

        repository.AddPerformanceReview(review);
        repository.SaveChanges();
    }

    public List<PerformanceReviewDto> GetPerformanceReviews(Guid managerUserId)
    {
        var manager = repository.GetEmployeeByUserId(managerUserId);

        if (manager == null)
        {
            throw new Exception("Manager not found");
        }

        return repository.GetPerformanceReviews(manager.Id)
            .Select(p => new PerformanceReviewDto
            {
                Id = p.Id,
                EmployeeName = p.Employee!.FirstName + " " + p.Employee.LastName,
                Rating = p.Rating,
                Comments = p.Comments,
                ReviewDate = p.ReviewDate
            })
            .ToList();
    }

    public List<PerformanceReviewDto> GetEmployeePerformanceReviews(Guid managerUserId, Guid employeeId)
    {
        var manager = repository.GetEmployeeByUserId(managerUserId);

        if (manager == null)
        {
            throw new Exception("Manager not found");
        }

        return repository.GetEmployeePerformanceReviews(manager.Id, employeeId)
            .Select(p => new PerformanceReviewDto
            {
                Id = p.Id,
                EmployeeName = p.Employee!.FirstName + " " + p.Employee.LastName,
                Rating = p.Rating,
                Comments = p.Comments,
                ReviewDate = p.ReviewDate
            })
            .ToList();
    }

    public EligibleEmployeesResponseDto GetEligibleEmployees(Guid managerUserId, string? search, int page, int pageSize)
    {
        var manager = repository.GetEmployeeByUserId(managerUserId);
        if (manager == null)
        {
            throw new NotFoundException("Manager not found.");
        }

        var (employees, totalCount) = repository.GetEligibleEmployees(manager.Id, search, page, pageSize);

        var list = employees.Select(e => new EligibleEmployeeDto
        {
            Id = e.Id,
            EmployeeCode = e.EmployeeCode,
            FullName = $"{e.FirstName} {e.LastName}",
            Email = e.Email,
            Designation = e.Designation ?? string.Empty,
            Department = e.Department?.Name ?? string.Empty,
            CurrentManagerName = e.Manager != null ? $"{e.Manager.FirstName} {e.Manager.LastName}" : null
        }).ToList();

        return new EligibleEmployeesResponseDto
        {
            Employees = list,
            TotalCount = totalCount
        };
    }

    public void AddTeamMember(Guid managerUserId, Guid employeeId)
    {
        var manager = repository.GetEmployeeByUserId(managerUserId);
        if (manager == null)
        {
            throw new NotFoundException("Manager not found.");
        }

        var employee = repository.GetEmployeeById(employeeId);
        if (employee == null)
        {
            throw new NotFoundException("Employee not found.");
        }

        if (employee.IsDeleted)
        {
            throw new BusinessException("Cannot assign a deleted employee.");
        }

        if (employee.EmploymentStatus != "Active" && employee.EmploymentStatus != "Probation" && employee.EmploymentStatus != "OnLeave")
        {
            throw new BusinessException($"Only active employees can be assigned to a manager. Current status: {employee.EmploymentStatus}.");
        }

        if (employee.Id == manager.Id)
        {
            throw new BusinessException("Managers cannot assign themselves to their own team.");
        }

        if (employee.User != null && employee.User.Roles.Any(r => r.Name == "Admin" || r.Name == "HR" || r.Name == "Manager"))
        {
            throw new BusinessException("Managers can only manage employees. Admins, HR representatives, and other Managers cannot be assigned.");
        }

        if (employee.ManagerId == manager.Id)
        {
            throw new BusinessException("Employee is already assigned to your team.");
        }

        if (IsCircularReporting(manager, employee.Id))
        {
            throw new BusinessException("Invalid assignment: this assignment would create a circular reporting structure.");
        }

        var currentTeamCount = repository.GetTeamMembers(manager.Id).Count;
        if (currentTeamCount >= 50)
        {
            throw new BusinessException("Maximum team size limit of 50 members reached.");
        }

        var oldManagerId = employee.ManagerId;
        string? oldManagerName = null;
        if (oldManagerId != null)
        {
            var oldManager = repository.GetEmployeeById(oldManagerId.Value);
            if (oldManager != null)
            {
                oldManagerName = $"{oldManager.FirstName} {oldManager.LastName}";
            }
        }

        // Perform assignment
        employee.ManagerId = manager.Id;
        repository.SaveChanges();

        // Audit log
        var details = $"Employee {employee.FirstName} {employee.LastName} ({employee.EmployeeCode}) assigned to Manager {manager.FirstName} {manager.LastName}.";
        if (oldManagerId != null)
        {
            details += $" Transferred from previous Manager {oldManagerName} (ID: {oldManagerId}).";
        }
        _ = auditLogService.LogAsync("AddTeamMember", "Employee", employee.Id, details);

        // Notify employee
        if (employee.UserId != null)
        {
            try
            {
                notificationService.CreateNotification(
                    employee.UserId.Value,
                    "Manager Assigned",
                    $"You have been assigned to Manager {manager.FirstName} {manager.LastName}."
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to notify employee: {ex.Message}");
            }
        }

        // Notify previous manager if transferred
        if (oldManagerId != null)
        {
            var oldMgr = repository.GetEmployeeById(oldManagerId.Value);
            if (oldMgr != null && oldMgr.UserId != null)
            {
                try
                {
                    notificationService.CreateNotification(
                        oldMgr.UserId.Value,
                        "Employee Transferred",
                        $"Employee {employee.FirstName} {employee.LastName} has been transferred to Manager {manager.FirstName} {manager.LastName}."
                    );
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to notify old manager: {ex.Message}");
                }
            }
        }
    }

    public void RemoveTeamMember(Guid managerUserId, Guid employeeId)
    {
        var manager = repository.GetEmployeeByUserId(managerUserId);
        if (manager == null)
        {
            throw new NotFoundException("Manager not found.");
        }

        var employee = repository.GetEmployeeById(employeeId);
        if (employee == null)
        {
            throw new NotFoundException("Employee not found.");
        }

        if (employee.ManagerId != manager.Id)
        {
            throw new ForbiddenException("You can only remove employees from your own team.");
        }

        // Perform removal
        employee.ManagerId = null;
        repository.SaveChanges();

        // Audit log
        var details = $"Employee {employee.FirstName} {employee.LastName} ({employee.EmployeeCode}) removed from Manager {manager.FirstName} {manager.LastName}'s team.";
        _ = auditLogService.LogAsync("RemoveTeamMember", "Employee", employee.Id, details);

        // Notify employee
        if (employee.UserId != null)
        {
            try
            {
                notificationService.CreateNotification(
                    employee.UserId.Value,
                    "Team Assignment Removed",
                    $"You have been removed from Manager {manager.FirstName} {manager.LastName}'s team."
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to notify employee: {ex.Message}");
            }
        }
    }

    private bool IsCircularReporting(Employee newManager, Guid employeeId)
    {
        if (newManager.Id == employeeId) return true;

        var current = newManager;
        while (current.ManagerId != null)
        {
            if (current.ManagerId == employeeId) return true;
            
            current = repository.GetEmployeeById(current.ManagerId.Value);
            if (current == null) break;
        }
        return false;
    }
}
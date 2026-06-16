using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Manager;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

public class ManagerService : IManagerService
{
    private readonly IManagerRepository repository;

    public ManagerService(IManagerRepository repository)
    {
        this.repository = repository;
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

        if (dto.Rating < 1 || dto.Rating > 5)
        {
            throw new Exception("Rating must be between 1 and 5");
        }

        PerformanceReview review = new()
        {
            Id = Guid.NewGuid(),
            EmployeeId = dto.EmployeeId,
            ReviewerId = manager.Id,
            Rating = dto.Rating,
            Comments = dto.Comments,
            ReviewDate = DateOnly.FromDateTime(DateTime.Today)
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
}
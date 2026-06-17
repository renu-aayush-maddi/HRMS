using System;
using System.Collections.Generic;

namespace HRMS.API.Models.Entities;

public partial class Employee
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string EmployeeCode { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Designation { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? ManagerId { get; set; }

    public DateOnly JoiningDate { get; set; }

    public string? EmploymentStatus { get; set; }

    public decimal? Salary { get; set; }

    public string? ProfilePhotoUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public Guid? DeletedBy { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public virtual ICollection<AttendanceLog> AttendanceLogs { get; set; } = new List<AttendanceLog>();

    public virtual ICollection<AttendanceRegularization> AttendanceRegularizationEmployees { get; set; } = new List<AttendanceRegularization>();

    public virtual ICollection<AttendanceRegularization> AttendanceRegularizationReviewedByNavigations { get; set; } = new List<AttendanceRegularization>();

    public virtual ICollection<Bonuse> Bonuses { get; set; } = new List<Bonuse>();

    public virtual ICollection<Deduction> Deductions { get; set; } = new List<Deduction>();

    public virtual Department? Department { get; set; }

    public virtual ICollection<EmployeeAddress> EmployeeAddresses { get; set; } = new List<EmployeeAddress>();

    public virtual ICollection<EmployeeDocument> EmployeeDocuments { get; set; } = new List<EmployeeDocument>();

    public virtual ICollection<EmployeeEducation> EmployeeEducations { get; set; } = new List<EmployeeEducation>();

    public virtual ICollection<EmployeeEmergencyContact> EmployeeEmergencyContacts { get; set; } = new List<EmployeeEmergencyContact>();

    public virtual ICollection<EmployeeExperience> EmployeeExperiences { get; set; } = new List<EmployeeExperience>();

    public virtual ICollection<EmployeeGoal> EmployeeGoalAssignedByNavigations { get; set; } = new List<EmployeeGoal>();

    public virtual ICollection<EmployeeGoal> EmployeeGoalEmployees { get; set; } = new List<EmployeeGoal>();

    public virtual ICollection<EmployeeLeaveBalance> EmployeeLeaveBalances { get; set; } = new List<EmployeeLeaveBalance>();

    public virtual ICollection<EmployeeResignation> EmployeeResignationApprovedByNavigations { get; set; } = new List<EmployeeResignation>();

    public virtual ICollection<EmployeeResignation> EmployeeResignationEmployees { get; set; } = new List<EmployeeResignation>();

    public virtual ICollection<EmployeeResignation> EmployeeResignationRejectedByNavigations { get; set; } = new List<EmployeeResignation>();

    public virtual ICollection<EmployeeSalary> EmployeeSalaries { get; set; } = new List<EmployeeSalary>();

    public virtual ICollection<Employee> InverseManager { get; set; } = new List<Employee>();

    public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();

    public virtual Employee? Manager { get; set; }

    public virtual ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();

    public virtual ICollection<PerformanceBonusRecommendation> PerformanceBonusRecommendations { get; set; } = new List<PerformanceBonusRecommendation>();

    public virtual ICollection<PerformanceReview> PerformanceReviewEmployees { get; set; } = new List<PerformanceReview>();

    public virtual ICollection<PerformanceReview> PerformanceReviewReviewers { get; set; } = new List<PerformanceReview>();

    public virtual User? User { get; set; }
}

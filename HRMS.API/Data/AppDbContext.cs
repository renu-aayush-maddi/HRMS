using System;
using System.Collections.Generic;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AttendanceLog> AttendanceLogs { get; set; }

    public virtual DbSet<AttendanceRegularization> AttendanceRegularizations { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Bonuse> Bonuses { get; set; }

    public virtual DbSet<Deduction> Deductions { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeAddress> EmployeeAddresses { get; set; }

    public virtual DbSet<EmployeeDocument> EmployeeDocuments { get; set; }

    public virtual DbSet<EmployeeEducation> EmployeeEducations { get; set; }

    public virtual DbSet<EmployeeEmergencyContact> EmployeeEmergencyContacts { get; set; }

    public virtual DbSet<EmployeeExperience> EmployeeExperiences { get; set; }

    public virtual DbSet<EmployeeGoal> EmployeeGoals { get; set; }

    public virtual DbSet<EmployeeLeaveBalance> EmployeeLeaveBalances { get; set; }

    public virtual DbSet<EmployeeResignation> EmployeeResignations { get; set; }

    public virtual DbSet<EmployeeSalary> EmployeeSalaries { get; set; }

    public virtual DbSet<Holiday> Holidays { get; set; }

    public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }

    public virtual DbSet<LeaveType> LeaveTypes { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payroll> Payrolls { get; set; }

    public virtual DbSet<PerformanceBonusRecommendation> PerformanceBonusRecommendations { get; set; }

    public virtual DbSet<PerformanceBonusRule> PerformanceBonusRules { get; set; }

    public virtual DbSet<PerformanceCycle> PerformanceCycles { get; set; }

    public virtual DbSet<PerformanceReview> PerformanceReviews { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SalaryStructure> SalaryStructures { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5469;Database=HRMS;Username=postgres;Password=aayush05");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<AttendanceLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("attendance_logs_pkey");

            entity.ToTable("attendance_logs");

            entity.HasIndex(e => new { e.EmployeeId, e.AttendanceDate }, "attendance_logs_employee_id_attendance_date_key").IsUnique();

            entity.HasIndex(e => new { e.EmployeeId, e.AttendanceDate }, "idx_attendance_employee_date");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AttendanceDate).HasColumnName("attendance_date");
            entity.Property(e => e.CheckIn)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("check_in");
            entity.Property(e => e.CheckOut)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("check_out");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Remarks)
                .HasMaxLength(500)
                .HasColumnName("remarks");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Present'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.WorkingHours)
                .HasPrecision(5, 2)
                .HasColumnName("working_hours");

            entity.HasOne(d => d.Employee).WithMany(p => p.AttendanceLogs)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("attendance_logs_employee_id_fkey");
        });

        modelBuilder.Entity<AttendanceRegularization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("attendance_regularizations_pkey");

            entity.ToTable("attendance_regularizations");

            entity.HasIndex(e => new { e.EmployeeId, e.AttendanceDate }, "ux_attendance_regularization_pending")
                .IsUnique()
                .HasFilter("((status)::text = 'Pending'::text)");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AttendanceDate).HasColumnName("attendance_date");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.HrComments).HasColumnName("hr_comments");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.RequestedCheckIn)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("requested_check_in");
            entity.Property(e => e.RequestedCheckOut)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("requested_check_out");
            entity.Property(e => e.ReviewedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("reviewed_at");
            entity.Property(e => e.ReviewedBy).HasColumnName("reviewed_by");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");

            entity.HasOne(d => d.Employee).WithMany(p => p.AttendanceRegularizationEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("attendance_regularizations_employee_id_fkey");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.AttendanceRegularizationReviewedByNavigations)
                .HasForeignKey(d => d.ReviewedBy)
                .HasConstraintName("fk_attendance_regularization_reviewed_by");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("audit_logs_pkey");

            entity.ToTable("audit_logs");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .HasColumnName("action");
            entity.Property(e => e.Details).HasColumnName("details");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(100)
                .HasColumnName("entity_type");
            entity.Property(e => e.PerformedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("performed_at");
            entity.Property(e => e.PerformedBy).HasColumnName("performed_by");
        });

        modelBuilder.Entity<Bonuse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bonuses_pkey");

            entity.ToTable("bonuses");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(12, 2)
                .HasColumnName("amount");
            entity.Property(e => e.BonusMonth).HasColumnName("bonus_month");
            entity.Property(e => e.BonusYear).HasColumnName("bonus_year");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.IsProcessed)
                .HasDefaultValue(false)
                .HasColumnName("is_processed");
            entity.Property(e => e.Reason)
                .HasMaxLength(200)
                .HasColumnName("reason");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");

            entity.HasOne(d => d.Employee).WithMany(p => p.Bonuses)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("bonuses_employee_id_fkey");
        });

        modelBuilder.Entity<Deduction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("deductions_pkey");

            entity.ToTable("deductions");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasPrecision(12, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeductionMonth).HasColumnName("deduction_month");
            entity.Property(e => e.DeductionYear).HasColumnName("deduction_year");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.IsProcessed)
                .HasDefaultValue(false)
                .HasColumnName("is_processed");
            entity.Property(e => e.Reason)
                .HasMaxLength(200)
                .HasColumnName("reason");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");

            entity.HasOne(d => d.Employee).WithMany(p => p.Deductions)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("deductions_employee_id_fkey");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("departments_pkey");

            entity.ToTable("departments");

            entity.HasIndex(e => e.Name, "departments_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employees_pkey");

            entity.ToTable("employees");

            entity.HasIndex(e => e.Email, "employees_email_key").IsUnique();

            entity.HasIndex(e => e.EmployeeCode, "employees_employee_code_key").IsUnique();

            entity.HasIndex(e => e.UserId, "employees_user_id_key").IsUnique();

            entity.HasIndex(e => e.DepartmentId, "idx_employees_department");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy).HasColumnName("deleted_by");
            entity.Property(e => e.DepartmentId).HasColumnName("department_id");
            entity.Property(e => e.Designation)
                .HasMaxLength(100)
                .HasColumnName("designation");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.EmployeeCode)
                .HasMaxLength(20)
                .HasColumnName("employee_code");
            entity.Property(e => e.EmploymentStatus)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Active'::character varying")
                .HasColumnName("employment_status");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.JoiningDate).HasColumnName("joining_date");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.ProfilePhotoUrl).HasColumnName("profile_photo_url");
            entity.Property(e => e.Salary)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("salary");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("employees_department_id_fkey");

            entity.HasOne(d => d.Manager).WithMany(p => p.InverseManager)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("employees_manager_id_fkey");

            entity.HasOne(d => d.User).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("employees_user_id_fkey");
        });

        modelBuilder.Entity<EmployeeAddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_addresses_pkey");

            entity.ToTable("employee_addresses");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AddressLine1)
                .HasMaxLength(300)
                .HasColumnName("address_line1");
            entity.Property(e => e.AddressLine2)
                .HasMaxLength(300)
                .HasColumnName("address_line2");
            entity.Property(e => e.AddressType)
                .HasMaxLength(50)
                .HasColumnName("address_type");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("postal_code");
            entity.Property(e => e.State)
                .HasMaxLength(100)
                .HasColumnName("state");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeAddresses)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("employee_addresses_employee_id_fkey");
        });

        modelBuilder.Entity<EmployeeDocument>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_documents_pkey");

            entity.ToTable("employee_documents");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.DocumentName)
                .HasMaxLength(200)
                .HasColumnName("document_name");
            entity.Property(e => e.DocumentType)
                .HasMaxLength(100)
                .HasColumnName("document_type");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.FileUrl).HasColumnName("file_url");
            entity.Property(e => e.IsVerified)
                .HasDefaultValue(false)
                .HasColumnName("is_verified");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("uploaded_at");
            entity.Property(e => e.VerifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("verified_at");
            entity.Property(e => e.VerifiedBy).HasColumnName("verified_by");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeDocuments)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("employee_documents_employee_id_fkey");

            entity.HasOne(d => d.VerifiedByNavigation).WithMany(p => p.EmployeeDocuments)
                .HasForeignKey(d => d.VerifiedBy)
                .HasConstraintName("employee_documents_verified_by_fkey");
        });

        modelBuilder.Entity<EmployeeEducation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_educations_pkey");

            entity.ToTable("employee_educations");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Degree)
                .HasMaxLength(200)
                .HasColumnName("degree");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.GraduationYear).HasColumnName("graduation_year");
            entity.Property(e => e.InstitutionName)
                .HasMaxLength(250)
                .HasColumnName("institution_name");
            entity.Property(e => e.Percentage)
                .HasPrecision(5, 2)
                .HasColumnName("percentage");
            entity.Property(e => e.Specialization)
                .HasMaxLength(200)
                .HasColumnName("specialization");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeEducations)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("employee_educations_employee_id_fkey");
        });

        modelBuilder.Entity<EmployeeEmergencyContact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_emergency_contacts_pkey");

            entity.ToTable("employee_emergency_contacts");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.ContactName)
                .HasMaxLength(150)
                .HasColumnName("contact_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Relationship)
                .HasMaxLength(100)
                .HasColumnName("relationship");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeEmergencyContacts)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("employee_emergency_contacts_employee_id_fkey");
        });

        modelBuilder.Entity<EmployeeExperience>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_experiences_pkey");

            entity.ToTable("employee_experiences");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(200)
                .HasColumnName("company_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Designation)
                .HasMaxLength(150)
                .HasColumnName("designation");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Responsibilities).HasColumnName("responsibilities");
            entity.Property(e => e.StartDate).HasColumnName("start_date");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeExperiences)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("employee_experiences_employee_id_fkey");
        });

        modelBuilder.Entity<EmployeeGoal>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_goals_pkey");

            entity.ToTable("employee_goals");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AssignedBy).HasColumnName("assigned_by");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Assigned'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TargetDate).HasColumnName("target_date");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.EmployeeGoalAssignedByNavigations)
                .HasForeignKey(d => d.AssignedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employee_goals_assigned_by_fkey");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeGoalEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("employee_goals_employee_id_fkey");
        });

        modelBuilder.Entity<EmployeeLeaveBalance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_leave_balances_pkey");

            entity.ToTable("employee_leave_balances");

            entity.HasIndex(e => new { e.EmployeeId, e.LeaveTypeId }, "employee_leave_balances_employee_id_leave_type_id_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AllocatedDays).HasColumnName("allocated_days");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.LeaveTypeId).HasColumnName("leave_type_id");
            entity.Property(e => e.RemainingDays).HasColumnName("remaining_days");
            entity.Property(e => e.UsedDays)
                .HasDefaultValue(0)
                .HasColumnName("used_days");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeLeaveBalances)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("employee_leave_balances_employee_id_fkey");

            entity.HasOne(d => d.LeaveType).WithMany(p => p.EmployeeLeaveBalances)
                .HasForeignKey(d => d.LeaveTypeId)
                .HasConstraintName("employee_leave_balances_leave_type_id_fkey");
        });

        modelBuilder.Entity<EmployeeResignation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_resignations_pkey");

            entity.ToTable("employee_resignations");

            entity.HasIndex(e => e.ResignationDate, "idx_resignation_date");

            entity.HasIndex(e => e.EmployeeId, "idx_resignation_employee");

            entity.HasIndex(e => e.Status, "idx_resignation_status");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.ApprovedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("approved_at");
            entity.Property(e => e.ApprovedBy).HasColumnName("approved_by");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.FinalSettlementStatus)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("final_settlement_status");
            entity.Property(e => e.HrComments).HasColumnName("hr_comments");
            entity.Property(e => e.LastWorkingDate).HasColumnName("last_working_date");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.RejectedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("rejected_at");
            entity.Property(e => e.RejectedBy).HasColumnName("rejected_by");
            entity.Property(e => e.ResignationDate).HasColumnName("resignation_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.WithdrawnAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("withdrawn_at");

            entity.HasOne(d => d.ApprovedByNavigation).WithMany(p => p.EmployeeResignationApprovedByNavigations)
                .HasForeignKey(d => d.ApprovedBy)
                .HasConstraintName("fk_employee_resignations_approved_by");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeResignationEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("employee_resignations_employee_id_fkey");

            entity.HasOne(d => d.RejectedByNavigation).WithMany(p => p.EmployeeResignationRejectedByNavigations)
                .HasForeignKey(d => d.RejectedBy)
                .HasConstraintName("fk_employee_resignations_rejected_by");
        });

        modelBuilder.Entity<EmployeeSalary>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_salaries_pkey");

            entity.ToTable("employee_salaries");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AnnualCtc)
                .HasPrecision(12, 2)
                .HasColumnName("annual_ctc");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EffectiveFrom).HasColumnName("effective_from");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.SalaryStructureId).HasColumnName("salary_structure_id");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeSalaries)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("employee_salaries_employee_id_fkey");

            entity.HasOne(d => d.SalaryStructure).WithMany(p => p.EmployeeSalaries)
                .HasForeignKey(d => d.SalaryStructureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employee_salaries_salary_structure_id_fkey");
        });

        modelBuilder.Entity<Holiday>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("holidays_pkey");

            entity.ToTable("holidays");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.HolidayDate).HasColumnName("holiday_date");
            entity.Property(e => e.IsOptional)
                .HasDefaultValue(false)
                .HasColumnName("is_optional");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("leave_requests_pkey");

            entity.ToTable("leave_requests");

            entity.HasIndex(e => e.EmployeeId, "idx_leave_employee");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.FromDate).HasColumnName("from_date");
            entity.Property(e => e.LeaveTypeId).HasColumnName("leave_type_id");
            entity.Property(e => e.ManagerComments).HasColumnName("manager_comments");
            entity.Property(e => e.Reason).HasColumnName("reason");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.ToDate).HasColumnName("to_date");

            entity.HasOne(d => d.Employee).WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("leave_requests_employee_id_fkey");

            entity.HasOne(d => d.LeaveType).WithMany(p => p.LeaveRequests)
                .HasForeignKey(d => d.LeaveTypeId)
                .HasConstraintName("leave_requests_leave_type_id_fkey");
        });

        modelBuilder.Entity<LeaveType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("leave_types_pkey");

            entity.ToTable("leave_types");

            entity.HasIndex(e => e.Name, "leave_types_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.AnnualAllocation).HasColumnName("annual_allocation");
            entity.Property(e => e.CarryForwardAllowed)
                .HasDefaultValue(false)
                .HasColumnName("carry_forward_allowed");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MaxCarryForward)
                .HasDefaultValue(0)
                .HasColumnName("max_carry_forward");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.NegativeBalanceAllowed)
                .HasDefaultValue(false)
                .HasColumnName("negative_balance_allowed");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("notifications_pkey");

            entity.ToTable("notifications");

            entity.HasIndex(e => e.UserId, "idx_notifications_user");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("notifications_user_id_fkey");
        });

        modelBuilder.Entity<Payroll>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payrolls_pkey");

            entity.ToTable("payrolls");

            entity.HasIndex(e => e.EmployeeId, "idx_payroll_employee");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.BasicComponent)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("basic_component");
            entity.Property(e => e.BasicSalary)
                .HasPrecision(12, 2)
                .HasColumnName("basic_salary");
            entity.Property(e => e.Bonus)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("bonus");
            entity.Property(e => e.Deductions)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("deductions");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.GeneratedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("generated_at");
            entity.Property(e => e.HraComponent)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("hra_component");
            entity.Property(e => e.LopDays)
                .HasDefaultValue(0)
                .HasColumnName("lop_days");
            entity.Property(e => e.LopDeduction)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("lop_deduction");
            entity.Property(e => e.MedicalAllowanceComponent)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("medical_allowance_component");
            entity.Property(e => e.NetSalary)
                .HasPrecision(12, 2)
                .HasComputedColumnSql("((basic_salary + bonus) - deductions)", true)
                .HasColumnName("net_salary");
            entity.Property(e => e.PayMonth).HasColumnName("pay_month");
            entity.Property(e => e.PayYear).HasColumnName("pay_year");
            entity.Property(e => e.PresentDays)
                .HasDefaultValue(0)
                .HasColumnName("present_days");
            entity.Property(e => e.SpecialAllowanceComponent)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("special_allowance_component");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Generated'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.TravelAllowanceComponent)
                .HasPrecision(12, 2)
                .HasDefaultValueSql("0")
                .HasColumnName("travel_allowance_component");
            entity.Property(e => e.WorkingDays)
                .HasDefaultValue(0)
                .HasColumnName("working_days");

            entity.HasOne(d => d.Employee).WithMany(p => p.Payrolls)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("payrolls_employee_id_fkey");
        });

        modelBuilder.Entity<PerformanceBonusRecommendation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("performance_bonus_recommendations_pkey");

            entity.ToTable("performance_bonus_recommendations");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.ApprovedAmount)
                .HasPrecision(12, 2)
                .HasColumnName("approved_amount");
            entity.Property(e => e.AverageRating)
                .HasPrecision(3, 2)
                .HasColumnName("average_rating");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.PerformanceCycleId).HasColumnName("performance_cycle_id");
            entity.Property(e => e.RecommendedAmount)
                .HasPrecision(12, 2)
                .HasColumnName("recommended_amount");
            entity.Property(e => e.RecommendedPercentage)
                .HasPrecision(5, 2)
                .HasColumnName("recommended_percentage");
            entity.Property(e => e.Remarks).HasColumnName("remarks");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasDefaultValueSql("'Pending'::character varying")
                .HasColumnName("status");

            entity.HasOne(d => d.Employee).WithMany(p => p.PerformanceBonusRecommendations)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bonus_recommendation_employee");
        });

        modelBuilder.Entity<PerformanceBonusRule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("performance_bonus_rules_pkey");

            entity.ToTable("performance_bonus_rules");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.BonusPercentage)
                .HasPrecision(5, 2)
                .HasColumnName("bonus_percentage");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.MaxRating)
                .HasPrecision(3, 2)
                .HasColumnName("max_rating");
            entity.Property(e => e.MinRating)
                .HasPrecision(3, 2)
                .HasColumnName("min_rating");
        });

        modelBuilder.Entity<PerformanceCycle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("performance_cycles_pkey");

            entity.ToTable("performance_cycles");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValueSql("'Open'::character varying")
                .HasColumnName("status");
        });

        modelBuilder.Entity<PerformanceReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("performance_reviews_pkey");

            entity.ToTable("performance_reviews");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.Comments).HasColumnName("comments");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.PerformanceCycleId).HasColumnName("performance_cycle_id");
            entity.Property(e => e.Rating)
                .HasPrecision(2, 1)
                .HasColumnName("rating");
            entity.Property(e => e.ReviewDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("review_date");
            entity.Property(e => e.ReviewerId).HasColumnName("reviewer_id");

            entity.HasOne(d => d.Employee).WithMany(p => p.PerformanceReviewEmployees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("performance_reviews_employee_id_fkey");

            entity.HasOne(d => d.PerformanceCycle).WithMany(p => p.PerformanceReviews)
                .HasForeignKey(d => d.PerformanceCycleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_performance_cycle");

            entity.HasOne(d => d.Reviewer).WithMany(p => p.PerformanceReviewReviewers)
                .HasForeignKey(d => d.ReviewerId)
                .HasConstraintName("performance_reviews_reviewer_id_fkey");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refresh_tokens_pkey");

            entity.ToTable("refresh_tokens");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expires_at");
            entity.Property(e => e.IsRevoked)
                .HasDefaultValue(false)
                .HasColumnName("is_revoked");
            entity.Property(e => e.Token).HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("refresh_tokens_user_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "roles_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<SalaryStructure>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("salary_structures_pkey");

            entity.ToTable("salary_structures");

            entity.HasIndex(e => e.Name, "salary_structures_name_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.BasicPercentage)
                .HasPrecision(5, 2)
                .HasColumnName("basic_percentage");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.HraPercentage)
                .HasPrecision(5, 2)
                .HasColumnName("hra_percentage");
            entity.Property(e => e.MedicalAllowancePercentage)
                .HasPrecision(5, 2)
                .HasColumnName("medical_allowance_percentage");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.SpecialAllowancePercentage)
                .HasPrecision(5, 2)
                .HasColumnName("special_allowance_percentage");
            entity.Property(e => e.TravelAllowancePercentage)
                .HasPrecision(5, 2)
                .HasColumnName("travel_allowance_percentage");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "idx_users_email");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.LastLoginAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_login_at");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("user_roles_role_id_fkey"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("user_roles_user_id_fkey"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("user_roles_pkey");
                        j.ToTable("user_roles");
                        j.IndexerProperty<Guid>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<Guid>("RoleId").HasColumnName("role_id");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

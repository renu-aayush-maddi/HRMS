using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRMS.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    details = table.Column<string>(type: "text", nullable: true),
                    performed_by = table.Column<Guid>(type: "uuid", nullable: true),
                    performed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("audit_logs_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("departments_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "holidays",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    holiday_date = table.Column<DateOnly>(type: "date", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_optional = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("holidays_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "leave_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    annual_allocation = table.Column<int>(type: "integer", nullable: false),
                    carry_forward_allowed = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    max_carry_forward = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    negative_balance_allowed = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("leave_types_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "performance_bonus_rules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    min_rating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    max_rating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    bonus_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("performance_bonus_rules_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "performance_cycles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValueSql: "'Open'::character varying"),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("performance_cycles_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("roles_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "salary_structures",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    basic_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    hra_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    special_allowance_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    medical_allowance_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    travel_allowance_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("salary_structures_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    last_login_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("users_pkey", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "employees",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    employee_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    designation = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    department_id = table.Column<Guid>(type: "uuid", nullable: true),
                    manager_id = table.Column<Guid>(type: "uuid", nullable: true),
                    joining_date = table.Column<DateOnly>(type: "date", nullable: false),
                    employment_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "'Active'::character varying"),
                    salary = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "0"),
                    profile_photo_url = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    deleted_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("employees_pkey", x => x.id);
                    table.ForeignKey(
                        name: "employees_department_id_fkey",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "employees_manager_id_fkey",
                        column: x => x.manager_id,
                        principalTable: "employees",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "employees_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    message = table.Column<string>(type: "text", nullable: true),
                    is_read = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("notifications_pkey", x => x.id);
                    table.ForeignKey(
                        name: "notifications_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    token = table.Column<string>(type: "text", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("refresh_tokens_pkey", x => x.id);
                    table.ForeignKey(
                        name: "refresh_tokens_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("user_roles_pkey", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "user_roles_role_id_fkey",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "user_roles_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attendance_logs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    attendance_date = table.Column<DateOnly>(type: "date", nullable: false),
                    check_in = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    check_out = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, defaultValueSql: "'Present'::character varying"),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    working_hours = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("attendance_logs_pkey", x => x.id);
                    table.ForeignKey(
                        name: "attendance_logs_employee_id_fkey",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attendance_regularizations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attendance_date = table.Column<DateOnly>(type: "date", nullable: false),
                    requested_check_in = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    requested_check_out = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true, defaultValueSql: "'Pending'::character varying"),
                    hr_comments = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    reviewed_by = table.Column<Guid>(type: "uuid", nullable: true),
                    reviewed_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("attendance_regularizations_pkey", x => x.id);
                    table.ForeignKey(
                        name: "attendance_regularizations_employee_id_fkey",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_attendance_regularization_reviewed_by",
                        column: x => x.reviewed_by,
                        principalTable: "employees",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "bonuses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    bonus_month = table.Column<int>(type: "integer", nullable: false),
                    bonus_year = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true, defaultValueSql: "'Pending'::character varying"),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_processed = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("bonuses_pkey", x => x.id);
                    table.ForeignKey(
                        name: "bonuses_employee_id_fkey",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "deductions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    deduction_month = table.Column<int>(type: "integer", nullable: false),
                    deduction_year = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true, defaultValueSql: "'Pending'::character varying"),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_processed = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("deductions_pkey", x => x.id);
                    table.ForeignKey(
                        name: "deductions_employee_id_fkey",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee_addresses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    address_line1 = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    address_line2 = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    state = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    address_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("employee_addresses_pkey", x => x.id);
                    table.ForeignKey(
                        name: "employee_addresses_employee_id_fkey",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee_documents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    document_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    document_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    file_url = table.Column<string>(type: "text", nullable: false),
                    uploaded_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    is_verified = table.Column<bool>(type: "boolean", nullable: true, defaultValue: false),
                    verified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    verified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("employee_documents_pkey", x => x.id);
                    table.ForeignKey(
                        name: "employee_documents_employee_id_fkey",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "employee_documents_verified_by_fkey",
                        column: x => x.verified_by,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "employee_educations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    degree = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    specialization = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    institution_name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    graduation_year = table.Column<int>(type: "integer", nullable: true),
                    percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("employee_educations_pkey", x => x.id);
                    table.ForeignKey(
                        name: "employee_educations_employee_id_fkey",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee_emergency_contacts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    contact_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    relationship = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("employee_emergency_contacts_pkey", x => x.id);
                    table.ForeignKey(
                        name: "employee_emergency_contacts_employee_id_fkey",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee_experiences",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    designation = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    start_date = table.Column<DateOnly>(type: "date", nullable: true),
                    end_date = table.Column<DateOnly>(type: "date", nullable: true),
                    responsibilities = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("employee_experiences_pkey", x => x.id);
                    table.ForeignKey(
                        name: "employee_experiences_employee_id_fkey",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee_goals",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_by = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    target_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "'Assigned'::character varying"),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("employee_goals_pkey", x => x.id);
                    table.ForeignKey(
                        name: "employee_goals_assigned_by_fkey",
                        column: x => x.assigned_by,
                        principalTable: "employees",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "employee_goals_employee_id_fkey",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "employee_leave_balances",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    leave_type_id = table.Column<Guid>(type: "uuid", nullable: true),
                    allocated_days = table.Column<int>(type: "integer", nullable: false),
                    used_days = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    remaining_days = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("employee_leave_balances_pkey", x => x.id);
                    table.ForeignKey(
                        name: "employee_leave_balances_employee_id_fkey",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "employee_leave_balances_leave_type_id_fkey",
                        column: x => x.leave_type_id,
                        principalTable: "leave_types",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "employee_resignations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    resignation_date = table.Column<DateOnly>(type: "date", nullable: false),
                    last_working_date = table.Column<DateOnly>(type: "date", nullable: true),
                    reason = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "'Pending'::character varying"),
                    hr_comments = table.Column<string>(type: "text", nullable: true),
                    final_settlement_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, defaultValueSql: "'Pending'::character varying"),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    approved_by = table.Column<Guid>(type: "uuid", nullable: true),
                    approved_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    rejected_by = table.Column<Guid>(type: "uuid", nullable: true),
                    rejected_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    withdrawn_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("employee_resignations_pkey", x => x.id);
                    table.ForeignKey(
                        name: "employee_resignations_employee_id_fkey",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_employee_resignations_approved_by",
                        column: x => x.approved_by,
                        principalTable: "employees",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_employee_resignations_rejected_by",
                        column: x => x.rejected_by,
                        principalTable: "employees",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "employee_salaries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    salary_structure_id = table.Column<Guid>(type: "uuid", nullable: false),
                    annual_ctc = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("employee_salaries_pkey", x => x.id);
                    table.ForeignKey(
                        name: "employee_salaries_employee_id_fkey",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "employee_salaries_salary_structure_id_fkey",
                        column: x => x.salary_structure_id,
                        principalTable: "salary_structures",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "leave_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    from_date = table.Column<DateOnly>(type: "date", nullable: false),
                    to_date = table.Column<DateOnly>(type: "date", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true, defaultValueSql: "'Pending'::character varying"),
                    manager_comments = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    leave_type_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("leave_requests_pkey", x => x.id);
                    table.ForeignKey(
                        name: "leave_requests_employee_id_fkey",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "leave_requests_leave_type_id_fkey",
                        column: x => x.leave_type_id,
                        principalTable: "leave_types",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "payrolls",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    pay_month = table.Column<int>(type: "integer", nullable: false),
                    pay_year = table.Column<int>(type: "integer", nullable: false),
                    basic_salary = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    bonus = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "0"),
                    deductions = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "0"),
                    net_salary = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true, computedColumnSql: "((basic_salary + bonus) - deductions)", stored: true),
                    generated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    working_days = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    present_days = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    lop_days = table.Column<int>(type: "integer", nullable: true, defaultValue: 0),
                    lop_deduction = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "0"),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true, defaultValueSql: "'Generated'::character varying"),
                    basic_component = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "0"),
                    hra_component = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "0"),
                    special_allowance_component = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "0"),
                    medical_allowance_component = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "0"),
                    travel_allowance_component = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true, defaultValueSql: "0")
                },
                constraints: table =>
                {
                    table.PrimaryKey("payrolls_pkey", x => x.id);
                    table.ForeignKey(
                        name: "payrolls_employee_id_fkey",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "performance_bonus_recommendations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: false),
                    average_rating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    recommended_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    recommended_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    approved_amount = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    remarks = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, defaultValueSql: "'Pending'::character varying"),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    performance_cycle_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("performance_bonus_recommendations_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_bonus_recommendation_employee",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "performance_reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    employee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reviewer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    rating = table.Column<decimal>(type: "numeric(2,1)", precision: 2, scale: 1, nullable: true),
                    comments = table.Column<string>(type: "text", nullable: true),
                    review_date = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "CURRENT_DATE"),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    performance_cycle_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("performance_reviews_pkey", x => x.id);
                    table.ForeignKey(
                        name: "fk_performance_cycle",
                        column: x => x.performance_cycle_id,
                        principalTable: "performance_cycles",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "performance_reviews_employee_id_fkey",
                        column: x => x.employee_id,
                        principalTable: "employees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "performance_reviews_reviewer_id_fkey",
                        column: x => x.reviewer_id,
                        principalTable: "employees",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "attendance_logs_employee_id_attendance_date_key",
                table: "attendance_logs",
                columns: new[] { "employee_id", "attendance_date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_attendance_employee_date",
                table: "attendance_logs",
                columns: new[] { "employee_id", "attendance_date" });

            migrationBuilder.CreateIndex(
                name: "IX_attendance_regularizations_reviewed_by",
                table: "attendance_regularizations",
                column: "reviewed_by");

            migrationBuilder.CreateIndex(
                name: "ux_attendance_regularization_pending",
                table: "attendance_regularizations",
                columns: new[] { "employee_id", "attendance_date" },
                unique: true,
                filter: "((status)::text = 'Pending'::text)");

            migrationBuilder.CreateIndex(
                name: "IX_bonuses_employee_id",
                table: "bonuses",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_deductions_employee_id",
                table: "deductions",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "departments_name_key",
                table: "departments",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employee_addresses_employee_id",
                table: "employee_addresses",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_documents_employee_id",
                table: "employee_documents",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_documents_verified_by",
                table: "employee_documents",
                column: "verified_by");

            migrationBuilder.CreateIndex(
                name: "IX_employee_educations_employee_id",
                table: "employee_educations",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_emergency_contacts_employee_id",
                table: "employee_emergency_contacts",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_experiences_employee_id",
                table: "employee_experiences",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_goals_assigned_by",
                table: "employee_goals",
                column: "assigned_by");

            migrationBuilder.CreateIndex(
                name: "IX_employee_goals_employee_id",
                table: "employee_goals",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "employee_leave_balances_employee_id_leave_type_id_key",
                table: "employee_leave_balances",
                columns: new[] { "employee_id", "leave_type_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_employee_leave_balances_leave_type_id",
                table: "employee_leave_balances",
                column: "leave_type_id");

            migrationBuilder.CreateIndex(
                name: "idx_resignation_date",
                table: "employee_resignations",
                column: "resignation_date");

            migrationBuilder.CreateIndex(
                name: "idx_resignation_employee",
                table: "employee_resignations",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "idx_resignation_status",
                table: "employee_resignations",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_employee_resignations_approved_by",
                table: "employee_resignations",
                column: "approved_by");

            migrationBuilder.CreateIndex(
                name: "IX_employee_resignations_rejected_by",
                table: "employee_resignations",
                column: "rejected_by");

            migrationBuilder.CreateIndex(
                name: "IX_employee_salaries_employee_id",
                table: "employee_salaries",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_employee_salaries_salary_structure_id",
                table: "employee_salaries",
                column: "salary_structure_id");

            migrationBuilder.CreateIndex(
                name: "employees_email_key",
                table: "employees",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "employees_employee_code_key",
                table: "employees",
                column: "employee_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "employees_user_id_key",
                table: "employees",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_employees_department",
                table: "employees",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "IX_employees_manager_id",
                table: "employees",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "idx_leave_employee",
                table: "leave_requests",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_leave_requests_leave_type_id",
                table: "leave_requests",
                column: "leave_type_id");

            migrationBuilder.CreateIndex(
                name: "leave_types_name_key",
                table: "leave_types",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_notifications_user",
                table: "notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_payroll_employee",
                table: "payrolls",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_performance_bonus_recommendations_employee_id",
                table: "performance_bonus_recommendations",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_performance_reviews_employee_id",
                table: "performance_reviews",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_performance_reviews_performance_cycle_id",
                table: "performance_reviews",
                column: "performance_cycle_id");

            migrationBuilder.CreateIndex(
                name: "IX_performance_reviews_reviewer_id",
                table: "performance_reviews",
                column: "reviewer_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "roles_name_key",
                table: "roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "salary_structures_name_key",
                table: "salary_structures",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "idx_users_email",
                table: "users",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "users_email_key",
                table: "users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attendance_logs");

            migrationBuilder.DropTable(
                name: "attendance_regularizations");

            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "bonuses");

            migrationBuilder.DropTable(
                name: "deductions");

            migrationBuilder.DropTable(
                name: "employee_addresses");

            migrationBuilder.DropTable(
                name: "employee_documents");

            migrationBuilder.DropTable(
                name: "employee_educations");

            migrationBuilder.DropTable(
                name: "employee_emergency_contacts");

            migrationBuilder.DropTable(
                name: "employee_experiences");

            migrationBuilder.DropTable(
                name: "employee_goals");

            migrationBuilder.DropTable(
                name: "employee_leave_balances");

            migrationBuilder.DropTable(
                name: "employee_resignations");

            migrationBuilder.DropTable(
                name: "employee_salaries");

            migrationBuilder.DropTable(
                name: "holidays");

            migrationBuilder.DropTable(
                name: "leave_requests");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "payrolls");

            migrationBuilder.DropTable(
                name: "performance_bonus_recommendations");

            migrationBuilder.DropTable(
                name: "performance_bonus_rules");

            migrationBuilder.DropTable(
                name: "performance_reviews");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "salary_structures");

            migrationBuilder.DropTable(
                name: "leave_types");

            migrationBuilder.DropTable(
                name: "performance_cycles");

            migrationBuilder.DropTable(
                name: "employees");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "departments");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}

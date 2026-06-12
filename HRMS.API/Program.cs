using HRMS.API.Data;
using HRMS.API.Helpers;
using HRMS.API.Repositories;
using HRMS.API.Services;
using HRMS.API.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using HRMS.API.Data;
using HRMS.API.Helpers;
using HRMS.API.Repositories;
using HRMS.API.Services;
using HRMS.API.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc; 
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;
using HRMS.API.Middleware;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;
using HRMS.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services
.AddControllers()
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory =
        context =>
        {
            var errors =
                context.ModelState
                    .Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

            return new BadRequestObjectResult(
                new
                {
                    Message =
                        "Validation Failed",
                    Errors = errors
                });
        };
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type = SecuritySchemeType.Http,

            Scheme = "bearer",

            BearerFormat = "JWT",

            In = ParameterLocation.Header,

            Description = "Enter JWT Token"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },

                Array.Empty<string>()
            }
        });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<JwtHelper>();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddScoped<IAttendanceRepository,AttendanceRepository>();
builder.Services.AddScoped<IAttendanceService,AttendanceService>();

builder.Services.AddScoped<ILeaveRepository,LeaveRepository>();
builder.Services.AddScoped<ILeaveService,LeaveService>();

builder.Services.AddScoped<IPayrollRepository,PayrollRepository>();
builder.Services.AddScoped<IPayrollService,PayrollService>();


builder.Services.AddScoped<IReviewRepository,ReviewRepository>();
builder.Services.AddScoped<IReviewService,ReviewService>();

builder.Services.AddScoped<IDepartmentRepository,DepartmentRepository>();
builder.Services.AddScoped<IDepartmentService,DepartmentService>();

builder.Services.AddScoped<IHrDashboardRepository,HrDashboardRepository>();
builder.Services.AddScoped<IHrDashboardService,HrDashboardService>();

builder.Services.AddScoped<INotificationRepository,NotificationRepository>();
builder.Services.AddScoped<INotificationService,NotificationService>();

builder.Services.AddScoped<ILeaveTypeRepository,LeaveTypeRepository>();
builder.Services.AddScoped<ILeaveTypeService,LeaveTypeService>();

builder.Services.AddScoped<ILeaveBalanceRepository,LeaveBalanceRepository>();
builder.Services.AddScoped<ILeaveBalanceService,LeaveBalanceService>();

builder.Services.AddScoped<IUserManagementRepository,UserManagementRepository>();
builder.Services.AddScoped<IUserManagementService,UserManagementService>();

builder.Services.AddScoped<IHolidayRepository,HolidayRepository>();
builder.Services.AddScoped<IHolidayService,HolidayService>();

builder.Services.AddScoped<IEmployeeDocumentRepository,EmployeeDocumentRepository>();
builder.Services.AddScoped<IEmployeeDocumentService,EmployeeDocumentService>();

builder.Services.AddScoped<IEmployeeEducationRepository,EmployeeEducationRepository>();
builder.Services.AddScoped<IEmployeeEducationService,EmployeeEducationService>();

builder.Services.AddScoped<IEmployeeExperienceRepository,EmployeeExperienceRepository>();
builder.Services.AddScoped<IEmployeeExperienceService,EmployeeExperienceService>();

builder.Services.AddScoped<IEmployeeEmergencyContactRepository,EmployeeEmergencyContactRepository>();
builder.Services.AddScoped<IEmployeeEmergencyContactService,EmployeeEmergencyContactService>();

builder.Services.AddScoped<IEmployeeAddressRepository,EmployeeAddressRepository>();
builder.Services.AddScoped<IEmployeeAddressService,EmployeeAddressService>();

builder.Services.AddScoped<IResignationRepository,ResignationRepository>();
builder.Services.AddScoped<IResignationService,ResignationService>();

builder.Services.AddScoped<IManagerRepository,ManagerRepository>();
builder.Services.AddScoped<IManagerService,ManagerService>();


builder.Services.AddScoped<IGoalRepository, GoalRepository>();
builder.Services.AddScoped<IGoalService,GoalService>();


builder.Services.AddScoped<ISalaryStructureRepository,SalaryStructureRepository>();
builder.Services.AddScoped<ISalaryStructureService,SalaryStructureService>();

builder.Services.AddScoped<IEmployeeSalaryRepository,EmployeeSalaryRepository>();
builder.Services.AddScoped<IEmployeeSalaryService,EmployeeSalaryService>();

builder.Services.AddScoped<IBonusRepository,BonusRepository>();
builder.Services.AddScoped<IBonusService,BonusService>();


builder.Services.AddScoped<IDeductionRepository,DeductionRepository>();
builder.Services.AddScoped<IDeductionService,DeductionService>();

builder.Services.AddScoped<IPayslipService,PayslipService>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,

            ValidateAudience = true,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,

            ValidIssuer =
                builder.Configuration["Jwt:Issuer"],

            ValidAudience =
                builder.Configuration["Jwt:Audience"],

            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        builder.Configuration["Jwt:Key"]!))
        };
});

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider =
            new PhysicalFileProvider(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Uploads")),

        RequestPath =
            "/Uploads"
    });

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
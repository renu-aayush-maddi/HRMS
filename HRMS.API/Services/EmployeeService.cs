// using BCrypt.Net;
// using HRMS.API.Exceptions;
// using HRMS.API.Interfaces;
// using HRMS.API.Models.Common;
// using HRMS.API.Models.DTOs.Employee;
// using HRMS.API.Models.Entities;
// using HRMS.API.Validators;
// using HRMS.API.Models.DTOs.EmployeeAddress;
// using HRMS.API.Models.DTOs.EmployeeDocument;
// using HRMS.API.Models.DTOs.EmployeeEducation;
// using HRMS.API.Models.DTOs.EmployeeEmergencyContact;
// using HRMS.API.Models.DTOs.EmployeeExperience;

// namespace HRMS.API.Services;

// public class EmployeeService : IEmployeeService
// {
//     private readonly IEmployeeRepository employeeRepository;
//     private readonly ILeaveBalanceService leaveBalanceService;
//     private readonly INotificationService notificationService;
//     private readonly EmployeeValidator employeeValidator;

//     private readonly IEmailService emailService;

//     public EmployeeService(
//         IEmployeeRepository employeeRepository,
//         ILeaveBalanceService leaveBalanceService,
//         INotificationService notificationService,
//         EmployeeValidator employeeValidator,
//         IEmailService emailService)
//     {
//         this.employeeRepository = employeeRepository;
//         this.leaveBalanceService = leaveBalanceService;
//         this.notificationService = notificationService;
//         this.employeeValidator = employeeValidator;
//         this.emailService = emailService;
//     }

//     private string GenerateEmployeeCode()
//     {
//         return $"EMP-{DateTime.Now.Year}-{Random.Shared.Next(1000, 9999)}";
//     }

//     private string GenerateTemporaryPassword()
//     {
//         return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
//             .Replace("/", "A")
//             .Replace("+", "B")
//             .Substring(0, 12);
//     }

//     public async Task<PagedResult<EmployeeResponseDto>> GetAllEmployeesAsync(
//         string? search,
//         int page,
//         int pageSize)
//     {
//         var result = await employeeRepository.GetAllEmployeesAsync(
//             search,
//             page,
//             pageSize);

//         return new PagedResult<EmployeeResponseDto>
//         {
//             Data = result.Employees
//                 .Select(e => new EmployeeResponseDto
//                 {
//                     Id = e.Id,
//                     EmployeeCode = e.EmployeeCode,
//                     FirstName = e.FirstName,
//                     LastName = e.LastName,
//                     Email = e.Email,
//                     Phone = e.Phone,
//                     Designation = e.Designation,
//                     Department = e.Department?.Name ?? string.Empty,
//                     Salary = e.Salary,
//                     EmploymentStatus = e.EmploymentStatus
//                 })
//                 .ToList(),

//             Page = page,
//             PageSize = pageSize,
//             TotalRecords = result.TotalCount
//         };
//     }
//     public async Task<EmployeeResponseDto?> GetEmployeeByIdAsync(Guid id)
//     {
//         var employee = await employeeRepository.GetEmployeeByIdAsync(id);

//         if (employee == null)
//         {
//             throw new NotFoundException("Employee not found");
//         }

//         return new EmployeeResponseDto
//         {
//             Id = employee.Id,
//             EmployeeCode = employee.EmployeeCode,
//             FirstName = employee.FirstName,
//             LastName = employee.LastName,
//             Email = employee.Email,
//             Phone = employee.Phone,
//             Designation = employee.Designation,
//             Department = employee.Department?.Name ?? string.Empty,
//             Salary = employee.Salary,
//             EmploymentStatus = employee.EmploymentStatus
//         };
//     }
//     public async Task<EmployeeCreatedDto> AddEmployeeAsync(AddEmployeeDto dto)
//     {
//         await employeeValidator.ValidateCreateAsync(dto);

//         var role = await employeeRepository.GetRoleByNameAsync(dto.Role);

//         string temporaryPassword = GenerateTemporaryPassword();

//         var user = new User
//         {
//             Id = Guid.NewGuid(),
//             Email = dto.Email,
//             PasswordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword),
//             IsActive = true
//         };

//         user.Roles.Add(role);

//         await employeeRepository.AddUserAsync(user);

//         var employee = new Employee
//         {
//             Id = Guid.NewGuid(),
//             UserId = user.Id,
//             EmployeeCode = GenerateEmployeeCode(),
//             FirstName = dto.FirstName,
//             LastName = dto.LastName,
//             Email = dto.Email,
//             Phone = dto.Phone,
//             Designation = dto.Designation,
//             DepartmentId = dto.DepartmentId,
//             Salary = dto.Salary,
//             JoiningDate = DateOnly.FromDateTime(DateTime.Now),
//             EmploymentStatus = "Active",
//             CreatedAt = DateTime.Now
//         };

//         await employeeRepository.AddEmployeeAsync(employee);

//         await employeeRepository.SaveChangesAsync();

//         await leaveBalanceService.AllocateDefaultBalancesAsync(employee.Id);


//         var emailBody = $@"
//         <h2>Welcome to HRMS</h2>

//         <p>Dear {employee.FirstName} {employee.LastName},</p>

//         <p>Your employee account has been created successfully.</p>

//         <table border='1' cellpadding='8' cellspacing='0'>
//         <tr>
//         <td><strong>Email</strong></td>
//         <td>{dto.Email}</td>
//         </tr>

//         <tr>
//         <td><strong>Temporary Password</strong></td>
//         <td>{temporaryPassword}</td>
//         </tr>

//         <tr>
//         <td><strong>Employee Code</strong></td>
//         <td>{employee.EmployeeCode}</td>
//         </tr>

//         <tr>
//         <td><strong>Role</strong></td>
//         <td>{dto.Role}</td>
//         </tr>
//         </table>

//         <br/>

//         <p>Please login and change your password after your first login.</p>

//         <p>Regards,<br/>HR Team</p>";


//         try
//         {
//             await emailService.SendEmailAsync(dto.Email,"Welcome To HRMS",emailBody);
//         }
//         catch
//         {
//         }

//         notificationService.CreateNotification(
//             user.Id,
//             "Welcome to HRMS",
//             $"Your account has been created. Temporary password: {temporaryPassword}");

//         return new EmployeeCreatedDto
//         {
//             Message = "Employee Created Successfully",
//             Email = dto.Email,
//             TemporaryPassword = temporaryPassword
//         };
//     }

//     public async Task UpdateEmployeeAsync(Guid id, UpdateEmployeeDto dto)
//     {
//         await employeeValidator.ValidateUpdateAsync(dto);

//         var employee = await employeeRepository.GetEmployeeByIdAsync(id);

//         if (employee == null)
//         {
//             throw new NotFoundException("Employee not found");
//         }

//         employee.FirstName = dto.FirstName;
//         employee.LastName = dto.LastName;
//         employee.Phone = dto.Phone;
//         employee.Designation = dto.Designation;
//         employee.Salary = dto.Salary;
//         employee.UpdatedAt = DateTime.Now;

//         employeeRepository.UpdateEmployee(employee);

//         await employeeRepository.SaveChangesAsync();
//     }

//     public async Task DeleteEmployeeAsync(Guid id)
//     {
//         var employee = await employeeRepository.GetEmployeeByIdAsync(id);

//         if (employee == null)
//         {
//             throw new NotFoundException("Employee not found");
//         }

//         employeeRepository.SoftDeleteEmployee(
//             employee,
//             Guid.Empty);

//         await employeeRepository.SaveChangesAsync();
//     }

//     public async Task UpdateEmployeeStatusAsync(
//     Guid employeeId,
//     UpdateEmployeeStatusDto dto)
//     {
//         await employeeValidator.ValidateStatusAsync(dto.Status);

//         var employee =
//             await employeeRepository.GetEmployeeByIdAsync(employeeId);

//         if (employee == null)
//         {
//             throw new NotFoundException("Employee not found");
//         }

//         employee.EmploymentStatus = dto.Status;
//         employee.UpdatedAt = DateTime.Now;

//         employeeRepository.UpdateEmployee(employee);

//         await employeeRepository.SaveChangesAsync();

//         if (employee.UserId != null)
//         {
//             notificationService.CreateNotification(
//                 employee.UserId.Value,
//                 "Employment Status Updated",
//                 $"Your employment status has been changed to {dto.Status}");
//         }
//     }

//     public async Task<EmployeeFullProfileDto?> GetFullProfileAsync(
//     Guid employeeId)
//     {
//         var employee =
//             await employeeRepository.GetEmployeeFullProfileAsync(employeeId);

//         if (employee == null)
//         {
//             throw new NotFoundException("Employee not found");
//         }

//         return new EmployeeFullProfileDto
//         {
//             Id = employee.Id,
//             EmployeeCode = employee.EmployeeCode,
//             FirstName = employee.FirstName,
//             LastName = employee.LastName,
//             Email = employee.Email,
//             Phone = employee.Phone,
//             Designation = employee.Designation,
//             Department = employee.Department?.Name,
//             ManagerName = employee.Manager == null
//                 ? null
//                 : $"{employee.Manager.FirstName} {employee.Manager.LastName}",
//             EmploymentStatus = employee.EmploymentStatus,
//             Salary = employee.Salary,

//             Educations = employee.EmployeeEducations
//                 .Select(x => new EmployeeEducationResponseDto
//                 {
//                     Id = x.Id,
//                     Degree = x.Degree ?? string.Empty,
//                     Specialization = x.Specialization ?? string.Empty,
//                     InstitutionName = x.InstitutionName ?? string.Empty,
//                     GraduationYear = x.GraduationYear ?? 0,
//                     Percentage = x.Percentage
//                 })
//                 .ToList(),

//             Experiences = employee.EmployeeExperiences
//                 .Select(x => new EmployeeExperienceResponseDto
//                 {
//                     Id = x.Id,
//                     CompanyName = x.CompanyName ?? string.Empty,
//                     Designation = x.Designation ?? string.Empty,
//                     StartDate = x.StartDate,
//                     EndDate = x.EndDate,
//                     Responsibilities = x.Responsibilities
//                 })
//                 .ToList(),

//             EmergencyContacts = employee.EmployeeEmergencyContacts
//                 .Select(x => new EmployeeEmergencyContactResponseDto
//                 {
//                     Id = x.Id,
//                     ContactName = x.ContactName ?? string.Empty,
//                     Relationship = x.Relationship ?? string.Empty,
//                     Phone = x.Phone ?? string.Empty,
//                     Email = x.Email ?? string.Empty
//                 })
//                 .ToList(),

//             Addresses = employee.EmployeeAddresses
//                 .Select(x => new EmployeeAddressResponseDto
//                 {
//                     Id = x.Id,
//                     AddressLine1 = x.AddressLine1 ?? string.Empty,
//                     AddressLine2 = x.AddressLine2,
//                     City = x.City ?? string.Empty,
//                     State = x.State ?? string.Empty,
//                     Country = x.Country ?? string.Empty,
//                     PostalCode = x.PostalCode ?? string.Empty,
//                     AddressType = x.AddressType ?? string.Empty
//                 })
//                 .ToList(),

//             Documents = employee.EmployeeDocuments
//                 .Select(x => new EmployeeDocumentResponseDto
//                 {
//                     Id = x.Id,
//                     DocumentName = x.DocumentName,
//                     DocumentType = x.DocumentType ?? string.Empty,
//                     FileUrl = x.FileUrl,
//                     IsVerified = x.IsVerified,
//                     UploadedAt = x.UploadedAt
//                 })
//                 .ToList()
//         };
//     }
// }


using BCrypt.Net;
using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.Employee;
using HRMS.API.Models.Entities;
using HRMS.API.Validators;
using HRMS.API.Models.DTOs.EmployeeAddress;
using HRMS.API.Models.DTOs.EmployeeDocument;
using HRMS.API.Models.DTOs.EmployeeEducation;
using HRMS.API.Models.DTOs.EmployeeEmergencyContact;
using HRMS.API.Models.DTOs.EmployeeExperience;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;

namespace HRMS.API.Services;

public class EmployeeService : IEmployeeService
{

    private readonly IEmployeeRepository employeeRepository;
    private readonly ILeaveBalanceService leaveBalanceService;
    private readonly INotificationService notificationService;
    private readonly EmployeeValidator employeeValidator;
    private readonly IEmailService emailService;
    private readonly IAuditLogService auditLogService;
    private readonly IUserContextService userContextService;
    private readonly ILogger<EmployeeService> logger;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        ILeaveBalanceService leaveBalanceService,
        INotificationService notificationService,
        EmployeeValidator employeeValidator,
        IEmailService emailService,
        IAuditLogService auditLogService,
        IUserContextService userContextService,
        ILogger<EmployeeService> logger)
    {
        this.employeeRepository = employeeRepository;
        this.leaveBalanceService = leaveBalanceService;
        this.notificationService = notificationService;
        this.employeeValidator = employeeValidator;
        this.emailService = emailService;
        this.auditLogService = auditLogService;
        this.userContextService = userContextService;
        this.logger = logger;
    }

    private string GenerateTemporaryPassword()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("/", "A")
            .Replace("+", "B")
            .Substring(0, 12);
    }

    private string GenerateEmployeeCode(long employeeNumber)
    {
        return $"EMP{employeeNumber:D7}";
    }

    public async Task<EmployeeCreatedDto> AddEmployeeAsync(
        AddEmployeeDto dto,
        CancellationToken cancellationToken = default)
    {
        await employeeValidator.ValidateCreateAsync(dto);

        if (dto.ManagerId.HasValue)
        {
            var managerExists = await employeeRepository.ManagerExistsAsync(dto.ManagerId.Value, cancellationToken);
            if (!managerExists)
            {
                throw new BusinessException("Manager not found.");
            }
        }

        var role = await employeeRepository.GetRoleByNameAsync(dto.Role, cancellationToken);
        if (role == null)
        {
            throw new BusinessException("Role not found.");
        }

        var temporaryPassword = GenerateTemporaryPassword();

        await using var transaction = await employeeRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            var employeeNumber = await employeeRepository.GetNextEmployeeNumberAsync(cancellationToken);
            var employeeCode = GenerateEmployeeCode(employeeNumber);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword),
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            user.Roles.Add(role);
            await employeeRepository.AddUserAsync(user, cancellationToken);

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                EmployeeCode = employeeCode,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                Designation = dto.Designation,
                DepartmentId = dto.DepartmentId,
                ManagerId = dto.ManagerId,
                Salary = dto.Salary,
                JoiningDate = DateOnly.FromDateTime(DateTime.Now),
                EmploymentStatus = "Active",
                CreatedAt = DateTime.Now,
                CreatedBy = userContextService.GetUserId()
            };

            await employeeRepository.AddEmployeeAsync(employee, cancellationToken);
            await employeeRepository.SaveChangesAsync(cancellationToken);

            await leaveBalanceService.AllocateDefaultBalancesAsync(employee.Id);
            await transaction.CommitAsync(cancellationToken);

            await auditLogService.LogAsync(
                "Create",
                nameof(Employee),
                employee.Id,
                $"Employee {employee.EmployeeCode} created.",
                cancellationToken);

            // try
            // {
            //     var emailBody = $@"
            //     <h2>Welcome to HRMS</h2>
            //     <p>Dear {employee.FirstName} {employee.LastName},</p>
            //     <p>Your employee account has been created successfully.</p>
            //     <table border='1' cellpadding='8' cellspacing='0'>
            //         <tr><td><strong>Email</strong></td><td>{dto.Email}</td></tr>
            //         <tr><td><strong>Temporary Password</strong></td><td>{temporaryPassword}</td></tr>
            //         <tr><td><strong>Employee Code</strong></td><td>{employee.EmployeeCode}</td></tr>
            //         <tr><td><strong>Role</strong></td><td>{dto.Role}</td></tr>
            //     </table>
            //     <p>Please login and change your password.</p>";

            //     await emailService.SendEmailAsync(dto.Email, "Welcome To HRMS", emailBody);
            // }
            // catch (Exception ex)
            // {
            //     logger.LogError(ex, "Failed sending employee onboarding email.");
            // }

            notificationService.CreateNotification(
                user.Id,
                "Welcome to HRMS",
                $"Your account has been created. Temporary password: {temporaryPassword}");

            logger.LogInformation("Employee {EmployeeCode} created successfully.", employee.EmployeeCode);

            return new EmployeeCreatedDto
            {
                Message = "Employee created successfully.",
                Email = dto.Email,
                TemporaryPassword = temporaryPassword
            };
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<EmployeeResponseDto> UpdateEmployeeAsync(
    Guid employeeId,
    UpdateEmployeeDto dto,
    CancellationToken cancellationToken = default)
    {
        await employeeValidator.ValidateUpdateAsync(dto);

        var employee = await employeeRepository.GetEmployeeByIdAsync(employeeId, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException("Employee not found.");
        }

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Phone = dto.Phone;
        employee.Designation = dto.Designation;
        employee.Salary = dto.Salary;
        employee.UpdatedAt = DateTime.Now;
        employee.UpdatedBy = userContextService.GetUserId();

        employeeRepository.UpdateEmployee(employee);
        await employeeRepository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Update",
            nameof(Employee),
            employee.Id,
            $"Employee {employee.EmployeeCode} updated.",
            cancellationToken);

        logger.LogInformation("Employee {EmployeeId} updated.", employee.Id);

        return new EmployeeResponseDto
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Phone = employee.Phone,
            Designation = employee.Designation,
            Department = employee.Department?.Name ?? string.Empty,
            Salary = employee.Salary,
            EmploymentStatus = employee.EmploymentStatus,
            ManagerId = employee.ManagerId,
            ManagerName = employee.Manager == null
                ? null
                : $"{employee.Manager.FirstName} {employee.Manager.LastName}"
        };
    }

    public async Task DeleteEmployeeAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetEmployeeByIdAsync(employeeId, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException("Employee not found.");
        }

        if (employee.UserId == userContextService.GetUserId())
        {
            throw new BusinessException("You cannot delete your own account.");
        }

        employeeRepository.SoftDeleteEmployee(employee, userContextService.GetUserId());
        await employeeRepository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Delete",
            nameof(Employee),
            employee.Id,
            $"Employee {employee.EmployeeCode} deleted.",
            cancellationToken);

        logger.LogInformation("Employee {EmployeeId} deleted.", employee.Id);
    }


    public async Task<PagedResponse<EmployeeResponseDto>> GetEmployeesAsync(
    EmployeeFilterDto filter,
    CancellationToken cancellationToken = default)
    {
        var query = employeeRepository.GetEmployees();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim().ToLower();
            query = query.Where(x =>
                x.FirstName.ToLower().Contains(search) ||
                x.LastName.ToLower().Contains(search) ||
                x.Email.ToLower().Contains(search) ||
                x.EmployeeCode.ToLower().Contains(search) ||
                (x.Phone != null && x.Phone.ToLower().Contains(search)) ||
                (x.Designation != null && x.Designation.ToLower().Contains(search)));
        }

        if (filter.DepartmentId.HasValue)
        {
            query = query.Where(x => x.DepartmentId == filter.DepartmentId.Value);
        }

        if (filter.ManagerId.HasValue)
        {
            query = query.Where(x => x.ManagerId == filter.ManagerId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filter.EmploymentStatus))
        {
            query = query.Where(x => x.EmploymentStatus == filter.EmploymentStatus);
        }

        if (!string.IsNullOrWhiteSpace(filter.Designation))
        {
            query = query.Where(x => x.Designation == filter.Designation);
        }

        query = filter.SortBy?.ToLower() switch
        {
            "firstname" => filter.Descending
                ? query.OrderByDescending(x => x.FirstName)
                : query.OrderBy(x => x.FirstName),
            "joiningdate" => filter.Descending
                ? query.OrderByDescending(x => x.JoiningDate)
                : query.OrderBy(x => x.JoiningDate),
            "employeecode" => filter.Descending
                ? query.OrderByDescending(x => x.EmployeeCode)
                : query.OrderBy(x => x.EmployeeCode),
            _ => query.OrderBy(x => x.FirstName)
        };

        var totalRecords = await query.CountAsync(cancellationToken);

        var employees = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<EmployeeResponseDto>
        {
            Data = employees.Select(x => new EmployeeResponseDto
            {
                Id = x.Id,
                EmployeeCode = x.EmployeeCode,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Phone = x.Phone,
                Designation = x.Designation,
                Department = x.Department?.Name ?? string.Empty,
                Salary = x.Salary,
                EmploymentStatus = x.EmploymentStatus,
                ManagerId = x.ManagerId,
                ManagerName = x.Manager == null
                    ? null
                    : $"{x.Manager.FirstName} {x.Manager.LastName}"
            }),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize)
        };
    }


    public async Task<EmployeeResponseDto> GetEmployeeByIdAsync(
    Guid employeeId,
    CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetEmployeeByIdAsync(employeeId, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException("Employee not found.");
        }

        return new EmployeeResponseDto
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Phone = employee.Phone,
            Designation = employee.Designation,
            Department = employee.Department?.Name ?? string.Empty,
            Salary = employee.Salary,
            EmploymentStatus = employee.EmploymentStatus,
            ManagerId = employee.ManagerId,
            ManagerName = employee.Manager == null
                ? null
                : $"{employee.Manager.FirstName} {employee.Manager.LastName}"
        };
    }

    public async Task<EmployeeFullProfileDto> GetEmployeeFullProfileAsync(
    Guid employeeId,
    CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetEmployeeFullProfileAsync(employeeId, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException("Employee not found.");
        }

        return MapFullProfile(employee);
    }

    public async Task<EmployeeFullProfileDto> GetMyProfileAsync(
    CancellationToken cancellationToken = default)
    {
        var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
        if (employeeId == null)
        {
            throw new NotFoundException("Employee profile not found.");
        }

        var employee = await employeeRepository.GetEmployeeFullProfileAsync(employeeId.Value, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException("Employee not found.");
        }

        return MapFullProfile(employee);
    }

    public async Task<List<ManagerLookupDto>> GetManagersAsync(
    CancellationToken cancellationToken = default)
    {
        var managers = await employeeRepository.GetManagersAsync(cancellationToken);

        return managers
            .Select(x => new ManagerLookupDto
            {
                Id = x.Id,
                EmployeeCode = x.EmployeeCode,
                FullName = $"{x.FirstName} {x.LastName}",
                Designation = x.Designation
            })
            .OrderBy(x => x.FullName)
            .ToList();
    }

    private static EmployeeFullProfileDto MapFullProfile(Employee employee)
    {
        return new EmployeeFullProfileDto
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Phone = employee.Phone,
            Designation = employee.Designation,
            Department = employee.Department?.Name,
            ManagerName = employee.Manager == null
                ? null
                : $"{employee.Manager.FirstName} {employee.Manager.LastName}",
            EmploymentStatus = employee.EmploymentStatus,
            Salary = employee.Salary,
            ProfilePhotoUrl = employee.ProfilePhotoUrl,
            Role = employee.User?.Roles.FirstOrDefault()?.Name,
            JoiningDate = employee.JoiningDate,

            Educations = employee.EmployeeEducations
                .Select(x => new EmployeeEducationResponseDto
                {
                    Id = x.Id,
                    Degree = x.Degree ?? string.Empty,
                    Specialization = x.Specialization ?? string.Empty,
                    InstitutionName = x.InstitutionName ?? string.Empty,
                    GraduationYear = x.GraduationYear ?? 0,
                    Percentage = x.Percentage
                })
                .ToList(),

            Experiences = employee.EmployeeExperiences
                .Select(x => new EmployeeExperienceResponseDto
                {
                    Id = x.Id,
                    CompanyName = x.CompanyName ?? string.Empty,
                    Designation = x.Designation ?? string.Empty,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Responsibilities = x.Responsibilities
                })
                .ToList(),

            EmergencyContacts = employee.EmployeeEmergencyContacts
                .Select(x => new EmployeeEmergencyContactResponseDto
                {
                    Id = x.Id,
                    ContactName = x.ContactName ?? string.Empty,
                    Relationship = x.Relationship ?? string.Empty,
                    Phone = x.Phone ?? string.Empty,
                    Email = x.Email ?? string.Empty
                })
                .ToList(),

            Addresses = employee.EmployeeAddresses
                .Select(x => new EmployeeAddressResponseDto
                {
                    Id = x.Id,
                    AddressLine1 = x.AddressLine1 ?? string.Empty,
                    AddressLine2 = x.AddressLine2,
                    City = x.City ?? string.Empty,
                    State = x.State ?? string.Empty,
                    Country = x.Country ?? string.Empty,
                    PostalCode = x.PostalCode ?? string.Empty,
                    AddressType = x.AddressType ?? string.Empty
                })
                .ToList(),

            Documents = employee.EmployeeDocuments
                .Select(x => new EmployeeDocumentResponseDto
                {
                    Id = x.Id,
                    DocumentName = x.DocumentName,
                    DocumentType = x.DocumentType ?? string.Empty,
                    FileUrl = x.FileUrl,
                    IsVerified = x.IsVerified,
                    UploadedAt = x.UploadedAt
                })
                .ToList()
        };
    }

    public async Task UpdateEmployeeStatusAsync(
    Guid employeeId,
    UpdateEmployeeStatusDto dto,
    CancellationToken cancellationToken = default)
    {
        await employeeValidator.ValidateStatusAsync(dto.Status);

        var employee = await employeeRepository.GetEmployeeByIdAsync(employeeId, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException("Employee not found.");
        }

        employee.EmploymentStatus = dto.Status;
        employee.UpdatedAt = DateTime.Now;
        employee.UpdatedBy = userContextService.GetUserId();

        employeeRepository.UpdateEmployee(employee);
        await employeeRepository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Status Update",
            nameof(Employee),
            employee.Id,
            $"Status changed to {dto.Status}",
            cancellationToken);

        logger.LogInformation(
            "Employee {EmployeeId} status updated to {Status}",
            employee.Id,
            dto.Status);
    }

    public async Task<byte[]> ExportEmployeesAsync(
    EmployeeFilterDto filter,
    CancellationToken cancellationToken = default)
    {
        var employees = await GetEmployeesAsync(filter, cancellationToken);

        using var workbook = new ClosedXML.Excel.XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Employees");

        worksheet.Cell(1, 1).Value = "Employee Code";
        worksheet.Cell(1, 2).Value = "First Name";
        worksheet.Cell(1, 3).Value = "Last Name";
        worksheet.Cell(1, 4).Value = "Email";
        worksheet.Cell(1, 5).Value = "Phone";
        worksheet.Cell(1, 6).Value = "Department";
        worksheet.Cell(1, 7).Value = "Designation";
        worksheet.Cell(1, 8).Value = "Status";
        worksheet.Cell(1, 9).Value = "Manager";

        var row = 2;
        foreach (var employee in employees.Data)
        {
            worksheet.Cell(row, 1).Value = employee.EmployeeCode;
            worksheet.Cell(row, 2).Value = employee.FirstName;
            worksheet.Cell(row, 3).Value = employee.LastName;
            worksheet.Cell(row, 4).Value = employee.Email;
            worksheet.Cell(row, 5).Value = employee.Phone ?? string.Empty;
            worksheet.Cell(row, 6).Value = employee.Department;
            worksheet.Cell(row, 7).Value = employee.Designation ?? string.Empty;
            worksheet.Cell(row, 8).Value = employee.EmploymentStatus ?? string.Empty;
            worksheet.Cell(row, 9).Value = employee.ManagerName ?? string.Empty;
            row++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<EmployeeImportResultDto> ImportEmployeesAsync(
    IFormFile file,
    CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            throw new BusinessException("File is required.");
        }

        var result = new EmployeeImportResultDto();

        using var stream = file.OpenReadStream();
        using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);
        var rows = worksheet.RowsUsed().Skip(1);

        foreach (var row in rows)
        {
            try
            {
                var dto = new AddEmployeeDto
                {
                    FirstName = row.Cell(1).GetString(),
                    LastName = row.Cell(2).GetString(),
                    Email = row.Cell(3).GetString(),
                    Phone = row.Cell(4).GetString(),
                    Designation = row.Cell(5).GetString(),
                    DepartmentId = Guid.Parse(row.Cell(6).GetString()),
                    Salary = decimal.Parse(row.Cell(7).GetString()),
                    Role = row.Cell(8).GetString(),
                    ManagerId =Guid.TryParse(row.Cell(9).GetString(),
                    out var managerId)? managerId: (Guid?)null
                };

                await AddEmployeeAsync(dto, cancellationToken);
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.FailedCount++;
                result.Errors.Add($"Row {row.RowNumber()}: {ex.Message}");
            }
        }

       result.TotalRecords = result.SuccessCount + result.FailedCount;

        await auditLogService.LogAsync(
            "Import",
            nameof(Employee),
            Guid.Empty,
            $"Imported {result.SuccessCount} employees.",
            cancellationToken);

        return result;
    }

    public async Task UpdateMyProfileAsync(
        Guid userId,
        UpdateMyProfileDto dto,
        CancellationToken cancellationToken = default)
    {
        var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
        if (employeeId == null)
        {
            throw new NotFoundException("Employee profile not found.");
        }

        var employee = await employeeRepository.GetEmployeeByIdAsync(employeeId.Value, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException("Employee not found.");
        }

        employee.Phone = dto.Phone;
        employee.UpdatedAt = DateTime.Now;
        employee.UpdatedBy = userId;

        employeeRepository.UpdateEmployee(employee);
        await employeeRepository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Update",
            nameof(Employee),
            employee.Id,
            $"Employee {employee.EmployeeCode} updated phone: {dto.Phone}",
            cancellationToken);

        logger.LogInformation("Employee {EmployeeId} updated their own phone number.", employee.Id);
    }

    public async Task<string> UploadProfilePhotoAsync(
        Guid userId,
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            throw new BusinessException("No file uploaded.");
        }

        var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
        if (employeeId == null)
        {
            throw new NotFoundException("Employee profile not found.");
        }

        var employee = await employeeRepository.GetEmployeeByIdAsync(employeeId.Value, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException("Employee not found.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        if (!allowedExtensions.Contains(extension))
        {
            throw new BusinessException("Invalid file format. Supported formats: JPG, JPEG, PNG, WEBP.");
        }

        if (file.Length > 5 * 1024 * 1024)
        {
            throw new BusinessException("File size exceeds the limit of 5 MB.");
        }

        // Validate image magic numbers
        using (var stream = file.OpenReadStream())
        {
            var header = new byte[8];
            var read = await stream.ReadAsync(header, 0, header.Length, cancellationToken);
            bool isValid = false;

            if (read >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF) // JPEG/JPG
            {
                isValid = true;
            }
            else if (read >= 8 && header.Take(8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 })) // PNG
            {
                isValid = true;
            }
            else if (read >= 4 && header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46) // RIFF/WEBP
            {
                isValid = true;
            }

            if (!isValid)
            {
                throw new BusinessException("Invalid image content. The file is not a valid image.");
            }
        }

        // Create directory
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        Directory.CreateDirectory(uploadsFolder);

        // Delete old profile picture if exists on disk
        if (!string.IsNullOrEmpty(employee.ProfilePhotoUrl))
        {
            var oldFileName = Path.GetFileName(employee.ProfilePhotoUrl);
            var oldFilePath = Path.Combine(uploadsFolder, oldFileName);
            if (File.Exists(oldFilePath))
            {
                try
                {
                    File.Delete(oldFilePath);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to delete old profile photo at {Path}", oldFilePath);
                }
            }
        }

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream, cancellationToken);
        }

        var photoUrl = $"/Uploads/{fileName}";
        employee.ProfilePhotoUrl = photoUrl;
        employee.UpdatedAt = DateTime.Now;
        employee.UpdatedBy = userId;

        employeeRepository.UpdateEmployee(employee);
        await employeeRepository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Update Photo",
            nameof(Employee),
            employee.Id,
            $"Uploaded new profile photo: {photoUrl}",
            cancellationToken);

        logger.LogInformation("Employee {EmployeeId} uploaded a new profile photo.", employee.Id);

        return photoUrl;
    }

    public async Task DeleteProfilePhotoAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
        if (employeeId == null)
        {
            throw new NotFoundException("Employee profile not found.");
        }

        var employee = await employeeRepository.GetEmployeeByIdAsync(employeeId.Value, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException("Employee not found.");
        }

        if (string.IsNullOrEmpty(employee.ProfilePhotoUrl))
        {
            return;
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
        var fileName = Path.GetFileName(employee.ProfilePhotoUrl);
        var filePath = Path.Combine(uploadsFolder, fileName);

        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete profile photo file at {Path}", filePath);
            }
        }

        employee.ProfilePhotoUrl = null;
        employee.UpdatedAt = DateTime.Now;
        employee.UpdatedBy = userId;

        employeeRepository.UpdateEmployee(employee);
        await employeeRepository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Delete Photo",
            nameof(Employee),
            employee.Id,
            "Deleted profile photo",
            cancellationToken);

        logger.LogInformation("Employee {EmployeeId} deleted their profile photo.", employee.Id);
    }
}
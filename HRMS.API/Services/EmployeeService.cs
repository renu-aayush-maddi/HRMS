// using HRMS.API.Interfaces;
// using HRMS.API.Models.DTOs.Employee;
// using HRMS.API.Models.DTOs.EmployeeAddress;
// using HRMS.API.Models.DTOs.EmployeeDocument;
// using HRMS.API.Models.DTOs.EmployeeEducation;
// using HRMS.API.Models.DTOs.EmployeeEmergencyContact;
// using HRMS.API.Models.DTOs.EmployeeExperience;
// using HRMS.API.Models.Entities;

// namespace HRMS.API.Services;

// public class EmployeeService : IEmployeeService
// {
//     private readonly IEmployeeRepository employeeRepository;
//     private readonly ILeaveBalanceService leaveBalanceService;
//     private readonly INotificationService notificationService;

//     public EmployeeService(IEmployeeRepository employeeRepository, ILeaveBalanceService leaveBalanceService, INotificationService notificationService)
//     {
//         this.employeeRepository = employeeRepository;
//         this.leaveBalanceService = leaveBalanceService;
//         this.notificationService = notificationService;
//     }

//     public List<EmployeeResponseDto> GetAllEmployees(string? search, int page, int pageSize)
//     {
//         return employeeRepository
//             .GetAllEmployees(search, page, pageSize)
//             .Select(e => new EmployeeResponseDto
//             {
//                 Id = e.Id,

//                 EmployeeCode = e.EmployeeCode,

//                 FirstName = e.FirstName,

//                 LastName = e.LastName,

//                 Email = e.Email,

//                 Phone = e.Phone,

//                 Designation = e.Designation,

//                 Department = e.Department?.Name ?? "",

//                 Salary = e.Salary,

//                 EmploymentStatus = e.EmploymentStatus
//             })
//             .ToList();
//     }

//     public EmployeeResponseDto? GetEmployeeById(Guid id)
//     {
//         var employee =
//             employeeRepository.GetEmployeeById(id);

//         if (employee == null)
//         {
//             return null;
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

//             Department = employee.Department?.Name ?? "",

//             Salary = employee.Salary,

//             EmploymentStatus = employee.EmploymentStatus
//         };
//     }

//     public EmployeeCreatedDto AddEmployee(AddEmployeeDto dto)
//     {
//         bool exists =
//             employeeRepository.EmployeeExists(dto.Email);

//         if (exists)
//         {
//             throw new Exception("Employee already exists");
//         }

//         var role =
//             employeeRepository.GetRoleByName(dto.Role);

//         if (role == null)
//         {
//             throw new Exception("Invalid role");
//         }

//         string temporaryPassword =
//             GenerateTemporaryPassword();

//         User user = new User
//         {
//             Id = Guid.NewGuid(),

//             Email = dto.Email,

//             PasswordHash =
//                 BCrypt.Net.BCrypt.HashPassword(
//                     temporaryPassword),

//             IsActive = true
//         };

//         user.Roles.Add(role);

//         employeeRepository.AddUser(user);

//         Employee employee = new Employee
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

//             JoiningDate =
//                 DateOnly.FromDateTime(DateTime.Now),

//             EmploymentStatus = "Active"
//         };

//         employeeRepository.AddEmployee(employee);

//         employeeRepository.SaveChanges();

//         leaveBalanceService.AllocateDefaultBalances(employee.Id);

//         notificationService.CreateNotification(user.Id, "Welcome to HRMS",
//     $"Your account has been created. Temporary password: {temporaryPassword}");

//         return new EmployeeCreatedDto
//         {
//             Message = "Employee Created Successfully",

//             Email = dto.Email,

//             TemporaryPassword = temporaryPassword
//         };
//     }

//     public void UpdateEmployee(
//         Guid id,
//         UpdateEmployeeDto dto)
//     {
//         var employee =
//             employeeRepository.GetEmployeeById(id);

//         if (employee == null)
//         {
//             throw new Exception("Employee not found");
//         }

//         employee.FirstName = dto.FirstName;

//         employee.LastName = dto.LastName;

//         employee.Phone = dto.Phone;

//         employee.Designation = dto.Designation;

//         employee.Salary = dto.Salary;

//         employeeRepository.UpdateEmployee(employee);

//         employeeRepository.SaveChanges();
//     }

//     public void DeleteEmployee(Guid id)
//     {
//         var employee =
//             employeeRepository.GetEmployeeById(id);

//         if (employee == null)
//         {
//             throw new Exception("Employee not found");
//         }

//         employeeRepository.DeleteEmployee(employee);

//         employeeRepository.SaveChanges();
//     }

//     private string GenerateEmployeeCode()
//     {
//         return "EMP" +
//                new Random().Next(1000, 9999);
//     }

//     private string GenerateTemporaryPassword()
//     {
//         return "HRMS@" +
//             new Random().Next(1000, 9999);
//     }

//     public void UpdateEmployeeStatus(Guid employeeId, UpdateEmployeeStatusDto dto)
//     {
//         var employee =
//             employeeRepository.GetEmployeeById(employeeId);

//         if (employee == null)
//         {
//             throw new Exception("Employee not found");
//         }

//         var validStatuses = new[]
//         {
//             "Active",
//             "Probation",
//             "OnLeave",
//             "NoticePeriod",
//             "Resigned",
//             "Terminated",
//             "Inactive"
//         };

//         if (!validStatuses.Contains(dto.Status))
//         {
//             throw new Exception("Invalid status");
//         }

//         employee.EmploymentStatus =
//             dto.Status;

//         employeeRepository.UpdateEmployee(employee);

//         employeeRepository.SaveChanges();

//         if (employee.UserId != null)
//         {
//             notificationService.CreateNotification(
//                 employee.UserId.Value,
//                 "Employment Status Updated",
//                 $"Your employment status has been changed to {dto.Status}");
//         }

//     }

//     public EmployeeFullProfileDto? GetFullProfile(Guid employeeId)
//     {
//         var employee =
//             employeeRepository
//             .GetEmployeeFullProfile(
//                 employeeId);

//         if (employee == null)
//         {
//             return null;
//         }

//         return new EmployeeFullProfileDto
//         {
//             Id = employee.Id,

//             EmployeeCode =
//                 employee.EmployeeCode,

//             FirstName =
//                 employee.FirstName,

//             LastName =
//                 employee.LastName,

//             Email =
//                 employee.Email,

//             Phone =
//                 employee.Phone,

//             Designation =
//                 employee.Designation,

//             Department =
//                 employee.Department?.Name,

//             ManagerName =
//                 employee.Manager == null
//                 ? null
//                 : employee.Manager.FirstName +
//                 " " +
//                 employee.Manager.LastName,

//             EmploymentStatus =
//                 employee.EmploymentStatus,

//             Salary =
//                 employee.Salary,

//             Educations =
//                 employee.EmployeeEducations
//                 .Select(x =>
//                     new EmployeeEducationResponseDto
//                     {
//                         Id = x.Id,

//                         Degree = x.Degree ?? "",

//                         Specialization =
//                             x.Specialization ?? "",

//                         InstitutionName =
//                             x.InstitutionName ?? "",

//                         GraduationYear =
//                             x.GraduationYear ?? 0,

//                         Percentage =
//                             x.Percentage
//                     })
//                 .ToList(),

//             Experiences =
//                 employee.EmployeeExperiences
//                 .Select(x =>
//                     new EmployeeExperienceResponseDto
//                     {
//                         Id = x.Id,

//                         CompanyName =
//                             x.CompanyName ?? "",

//                         Designation =
//                             x.Designation ?? "",

//                         StartDate =
//                             x.StartDate,

//                         EndDate =
//                             x.EndDate,

//                         Responsibilities =
//                             x.Responsibilities
//                     })
//                 .ToList(),

//             EmergencyContacts =
//                 employee.EmployeeEmergencyContacts
//                 .Select(x =>
//                     new EmployeeEmergencyContactResponseDto
//                     {
//                         Id = x.Id,

//                         ContactName =
//                             x.ContactName ?? "",

//                         Relationship =
//                             x.Relationship ?? "",

//                         Phone =
//                             x.Phone ?? "",

//                         Email =
//                             x.Email ?? ""
//                     })
//                 .ToList(),

//             Addresses =
//                 employee.EmployeeAddresses
//                 .Select(x =>
//                     new EmployeeAddressResponseDto
//                     {
//                         Id = x.Id,

//                         AddressLine1 =
//                             x.AddressLine1 ?? "",

//                         AddressLine2 =
//                             x.AddressLine2,

//                         City =
//                             x.City ?? "",

//                         State =
//                             x.State ?? "",

//                         Country =
//                             x.Country ?? "",

//                         PostalCode =
//                             x.PostalCode ?? "",

//                         AddressType =
//                             x.AddressType ?? ""
//                     })
//                 .ToList(),

//             Documents =
//                 employee.EmployeeDocuments
//                 .Select(x =>
//                     new EmployeeDocumentResponseDto
//                     {
//                         Id = x.Id,

//                         DocumentName =
//                             x.DocumentName,

//                         DocumentType =
//                             x.DocumentType ?? "",

//                         FileUrl =
//                             x.FileUrl,

//                         IsVerified =
//                             x.IsVerified,

//                         UploadedAt =
//                             x.UploadedAt
//                     })
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

namespace HRMS.API.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository employeeRepository;
    private readonly ILeaveBalanceService leaveBalanceService;
    private readonly INotificationService notificationService;
    private readonly EmployeeValidator employeeValidator;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        ILeaveBalanceService leaveBalanceService,
        INotificationService notificationService,
        EmployeeValidator employeeValidator)
    {
        this.employeeRepository = employeeRepository;
        this.leaveBalanceService = leaveBalanceService;
        this.notificationService = notificationService;
        this.employeeValidator = employeeValidator;
    }

    private string GenerateEmployeeCode()
    {
        return $"EMP-{DateTime.Now.Year}-{Random.Shared.Next(1000, 9999)}";
    }

    private string GenerateTemporaryPassword()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("/", "A")
            .Replace("+", "B")
            .Substring(0, 12);
    }

    public async Task<PagedResult<EmployeeResponseDto>> GetAllEmployeesAsync(
        string? search,
        int page,
        int pageSize)
    {
        var result = await employeeRepository.GetAllEmployeesAsync(
            search,
            page,
            pageSize);

        return new PagedResult<EmployeeResponseDto>
        {
            Data = result.Employees
                .Select(e => new EmployeeResponseDto
                {
                    Id = e.Id,
                    EmployeeCode = e.EmployeeCode,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    Phone = e.Phone,
                    Designation = e.Designation,
                    Department = e.Department?.Name ?? string.Empty,
                    Salary = e.Salary,
                    EmploymentStatus = e.EmploymentStatus
                })
                .ToList(),

            Page = page,
            PageSize = pageSize,
            TotalRecords = result.TotalCount
        };
    }
    public async Task<EmployeeResponseDto?> GetEmployeeByIdAsync(Guid id)
    {
        var employee = await employeeRepository.GetEmployeeByIdAsync(id);

        if (employee == null)
        {
            throw new NotFoundException("Employee not found");
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
            EmploymentStatus = employee.EmploymentStatus
        };
    }
    public async Task<EmployeeCreatedDto> AddEmployeeAsync(AddEmployeeDto dto)
    {
        await employeeValidator.ValidateCreateAsync(dto);

        var role = await employeeRepository.GetRoleByNameAsync(dto.Role);

        string temporaryPassword = GenerateTemporaryPassword();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword),
            IsActive = true
        };

        user.Roles.Add(role);

        await employeeRepository.AddUserAsync(user);

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            EmployeeCode = GenerateEmployeeCode(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Designation = dto.Designation,
            DepartmentId = dto.DepartmentId,
            Salary = dto.Salary,
            JoiningDate = DateOnly.FromDateTime(DateTime.Now),
            EmploymentStatus = "Active",
            CreatedAt = DateTime.Now
        };

        await employeeRepository.AddEmployeeAsync(employee);

        await employeeRepository.SaveChangesAsync();

        await leaveBalanceService.AllocateDefaultBalancesAsync(employee.Id);

        notificationService.CreateNotification(
            user.Id,
            "Welcome to HRMS",
            $"Your account has been created. Temporary password: {temporaryPassword}");

        return new EmployeeCreatedDto
        {
            Message = "Employee Created Successfully",
            Email = dto.Email,
            TemporaryPassword = temporaryPassword
        };
    }

    public async Task UpdateEmployeeAsync(Guid id,UpdateEmployeeDto dto)
    {
        await employeeValidator.ValidateUpdateAsync(dto);

        var employee = await employeeRepository.GetEmployeeByIdAsync(id);

        if (employee == null)
        {
            throw new NotFoundException("Employee not found");
        }

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Phone = dto.Phone;
        employee.Designation = dto.Designation;
        employee.Salary = dto.Salary;
        employee.UpdatedAt = DateTime.Now;

        employeeRepository.UpdateEmployee(employee);

        await employeeRepository.SaveChangesAsync();
    }

    public async Task DeleteEmployeeAsync(Guid id)
    {
        var employee = await employeeRepository.GetEmployeeByIdAsync(id);

        if (employee == null)
        {
            throw new NotFoundException("Employee not found");
        }

        employeeRepository.SoftDeleteEmployee(
            employee,
            Guid.Empty);

        await employeeRepository.SaveChangesAsync();
    }

    public async Task UpdateEmployeeStatusAsync(
    Guid employeeId,
    UpdateEmployeeStatusDto dto)
    {
        await employeeValidator.ValidateStatusAsync(dto.Status);

        var employee =
            await employeeRepository.GetEmployeeByIdAsync(employeeId);

        if (employee == null)
        {
            throw new NotFoundException("Employee not found");
        }

        employee.EmploymentStatus = dto.Status;
        employee.UpdatedAt = DateTime.Now;

        employeeRepository.UpdateEmployee(employee);

        await employeeRepository.SaveChangesAsync();

        if (employee.UserId != null)
        {
            notificationService.CreateNotification(
                employee.UserId.Value,
                "Employment Status Updated",
                $"Your employment status has been changed to {dto.Status}");
        }
    }

    public async Task<EmployeeFullProfileDto?> GetFullProfileAsync(
    Guid employeeId)
    {
        var employee =
            await employeeRepository.GetEmployeeFullProfileAsync(employeeId);

        if (employee == null)
        {
            throw new NotFoundException("Employee not found");
        }

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
}
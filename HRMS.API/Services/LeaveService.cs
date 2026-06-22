
// using HRMS.API.Exceptions;
// using HRMS.API.Interfaces;
// using HRMS.API.Models.DTOs.Leave;
// using HRMS.API.Models.Entities;
// using HRMS.API.Validators;

// namespace HRMS.API.Services;

// public class LeaveService : ILeaveService
// {
//     private readonly ILeaveRepository leaveRepository;
//     private readonly INotificationService notificationService;
//     private readonly LeaveValidator leaveValidator;

//     public LeaveService(ILeaveRepository leaveRepository, INotificationService notificationService, LeaveValidator leaveValidator)
//     {
//         this.leaveRepository = leaveRepository;
//         this.notificationService = notificationService;
//         this.leaveValidator = leaveValidator;
//     }

//     public async Task ApplyLeaveAsync(ApplyLeaveDto dto, Guid userId, string role)
//     {
//         await leaveValidator.ValidateApplyLeaveAsync(dto);

//         Guid employeeId;

//         if (role == "Employee" || role == "Manager")
//         {
//             var loggedInEmployee = await leaveRepository.GetEmployeeByUserIdAsync(userId);

//             if (loggedInEmployee == null)
//             {
//                 throw new NotFoundException("Employee not found");
//             }

//             employeeId = loggedInEmployee.Id;
//         }
//         else
//         {
//             if (dto.EmployeeId == null)
//             {
//                 throw new BusinessException("EmployeeId is required");
//             }

//             employeeId = dto.EmployeeId.Value;
//         }

//         var employee = await leaveRepository.GetEmployeeAsync(employeeId);

//         if (employee == null)
//         {
//             throw new NotFoundException("Employee not found");
//         }

//         var leaveType = await leaveRepository.GetLeaveTypeAsync(dto.LeaveTypeId);

//         if (leaveType == null)
//         {
//             throw new NotFoundException("Leave type not found");
//         }

//         var balance = await leaveRepository.GetLeaveBalanceAsync(employeeId, dto.LeaveTypeId);

//         if (balance == null)
//         {
//             throw new NotFoundException("Leave balance not allocated");
//         }

//         int days = dto.ToDate.DayNumber - dto.FromDate.DayNumber + 1;

//         if (balance.RemainingDays < days)
//         {
//             throw new BusinessException($"Insufficient leave balance. Remaining: {balance.RemainingDays}");
//         }

//         bool overlap = await leaveRepository.HasOverlappingLeaveAsync(employeeId, dto.FromDate, dto.ToDate);

//         if (overlap)
//         {
//             throw new BusinessException("Leave already exists for selected dates");
//         }

//         var leave = new LeaveRequest
//         {
//             Id = Guid.NewGuid(),
//             EmployeeId = employeeId,
//             LeaveTypeId = dto.LeaveTypeId,
//             FromDate = dto.FromDate,
//             ToDate = dto.ToDate,
//             Reason = dto.Reason,
//             Status = "Pending",
//             CreatedAt = DateTime.Now
//         };

//         await leaveRepository.AddLeaveAsync(leave);
//         await leaveRepository.SaveChangesAsync();
//     }

//     public async Task<List<LeaveResponseDto>> GetAllLeavesAsync()
//     {
//         var leaves = await leaveRepository.GetAllLeavesAsync();

//         return leaves.Select(l => new LeaveResponseDto
//         {
//             Id = l.Id,
//             EmployeeName = $"{l.Employee!.FirstName} {l.Employee.LastName}",
//             FromDate = l.FromDate,
//             ToDate = l.ToDate,
//             Reason = l.Reason,
//             Status = l.Status,
//             ManagerComments = l.ManagerComments,
//             LeaveType = l.LeaveType?.Name ?? string.Empty
//         }).ToList();
//     }

//     public async Task<List<LeaveResponseDto>> GetEmployeeLeavesAsync(Guid employeeId)
//     {
//         var leaves = await leaveRepository.GetEmployeeLeavesAsync(employeeId);

//         return leaves.Select(l => new LeaveResponseDto
//         {
//             Id = l.Id,
//             EmployeeName = $"{l.Employee!.FirstName} {l.Employee.LastName}",
//             FromDate = l.FromDate,
//             ToDate = l.ToDate,
//             Reason = l.Reason,
//             Status = l.Status,
//             ManagerComments = l.ManagerComments,
//             LeaveType = l.LeaveType?.Name ?? string.Empty
//         }).ToList();
//     }

//     public async Task<List<LeaveResponseDto>> GetMyLeavesAsync(Guid userId)
//     {
//         var employee = await leaveRepository.GetEmployeeByUserIdAsync(userId);

//         if (employee == null)
//         {
//             throw new NotFoundException("Employee not found");
//         }

//         return await GetEmployeeLeavesAsync(employee.Id);
//     }

//     public async Task ApproveLeaveAsync(Guid leaveId, Guid userId, string role, LeaveActionDto dto)
//     {
//         var leave = await leaveRepository.GetLeaveByIdAsync(leaveId);

//         if (leave == null)
//         {
//             throw new NotFoundException("Leave request not found");
//         }

//         if (role == "Manager")
//         {
//             var manager = await leaveRepository.GetEmployeeByUserIdAsync(userId);

//             if (manager == null)
//             {
//                 throw new NotFoundException("Manager not found");
//             }

//             if (leave.Employee?.ManagerId != manager.Id)
//             {
//                 throw new BusinessException("You are not authorized to approve this leave");
//             }
//         }

//         if (leave.Status != "Pending")
//         {
//             throw new BusinessException("Leave request already processed");
//         }

//         var balance = await leaveRepository.GetLeaveBalanceAsync(leave.EmployeeId!.Value, leave.LeaveTypeId!.Value);

//         if (balance == null)
//         {
//             throw new NotFoundException("Leave balance not found");
//         }

//         var leaveType = await leaveRepository.GetLeaveTypeAsync(leave.LeaveTypeId.Value);

//         if (leaveType == null)
//         {
//             throw new NotFoundException("Leave type not found");
//         }

//         int days = leave.ToDate.DayNumber - leave.FromDate.DayNumber + 1;

//         if (balance.RemainingDays < days && leaveType.NegativeBalanceAllowed != true)
//         {
//             throw new BusinessException("Insufficient leave balance");
//         }

//         balance.UsedDays += days;
//         balance.RemainingDays -= days;

//         leave.Status = "Approved";
//         leave.ManagerComments = dto.ManagerComments;

//         leaveRepository.UpdateLeaveBalance(balance);
//         leaveRepository.UpdateLeave(leave);

//         await leaveRepository.SaveChangesAsync();

//         if (leave.Employee?.UserId != null)
//         {
//             notificationService.CreateNotification(
//                 leave.Employee.UserId.Value,
//                 "Leave Approved",
//                 "Your leave request has been approved.");
//         }
//     }

//     public async Task RejectLeaveAsync(Guid leaveId, Guid userId, string role, LeaveActionDto dto)
//     {
//         var leave = await leaveRepository.GetLeaveByIdAsync(leaveId);

//         if (leave == null)
//         {
//             throw new NotFoundException("Leave request not found");
//         }

//         if (role == "Manager")
//         {
//             var manager = await leaveRepository.GetEmployeeByUserIdAsync(userId);

//             if (manager == null)
//             {
//                 throw new NotFoundException("Manager not found");
//             }

//             if (leave.Employee?.ManagerId != manager.Id)
//             {
//                 throw new BusinessException("You are not authorized to reject this leave");
//             }
//         }

//         if (leave.Status != "Pending")
//         {
//             throw new BusinessException("Leave request already processed");
//         }

//         leave.Status = "Rejected";
//         leave.ManagerComments = dto.ManagerComments;

//         leaveRepository.UpdateLeave(leave);

//         await leaveRepository.SaveChangesAsync();

//         if (leave.Employee?.UserId != null)
//         {
//             notificationService.CreateNotification(
//                 leave.Employee.UserId.Value,
//                 "Leave Rejected",
//                 "Your leave request has been rejected.");
//         }
//     }
// }

using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.Leave;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace HRMS.API.Services;

public class LeaveService : ILeaveService
{
    private readonly ILeaveRepository repository;
    private readonly IUserContextService userContextService;
    private readonly INotificationService notificationService;
    private readonly IAuditLogService auditLogService;
    private readonly ILogger<LeaveService> logger;

    public LeaveService(
        ILeaveRepository repository,
        IUserContextService userContextService,
        INotificationService notificationService,
        IAuditLogService auditLogService,
        ILogger<LeaveService> logger)
    {
        this.repository = repository;
        this.userContextService = userContextService;
        this.notificationService = notificationService;
        this.auditLogService = auditLogService;
        this.logger = logger;
    }


    public async Task ApplyLeaveAsync(
    ApplyLeaveDto dto,
    CancellationToken cancellationToken = default)
    {
        Guid employeeId;

        if (userContextService.IsAdminOrHr())
        {
            if (!dto.EmployeeId.HasValue)
            {
                throw new BusinessException("EmployeeId is required.");
            }

            employeeId = dto.EmployeeId.Value;
        }
        else
        {
            var loggedInEmployeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
            if (loggedInEmployeeId == null)
            {
                throw new NotFoundException("Employee profile not found.");
            }

            employeeId = loggedInEmployeeId.Value;
        }

        var employee = await repository.GetEmployeeAsync(employeeId, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException("Employee not found.");
        }

        var leaveType = await repository.GetLeaveTypeAsync(dto.LeaveTypeId, cancellationToken);
        if (leaveType == null)
        {
            throw new NotFoundException("Leave type not found.");
        }

        if (dto.FromDate > dto.ToDate)
        {
            throw new BusinessException("From date cannot be greater than to date.");
        }

        if (dto.FromDate < DateOnly.FromDateTime(DateTime.Today))
        {
            throw new BusinessException("Past leave cannot be applied.");
        }

        var overlap = await repository.HasOverlappingLeaveAsync(employeeId, dto.FromDate, dto.ToDate, cancellationToken);
        if (overlap)
        {
            throw new BusinessException("Leave already exists for selected dates.");
        }

        var balance = await repository.GetLeaveBalanceAsync(employeeId, dto.LeaveTypeId, cancellationToken);
        if (balance == null)
        {
            throw new BusinessException("Leave balance not allocated.");
        }

        var totalDays = dto.ToDate.DayNumber - dto.FromDate.DayNumber + 1;

        if (leaveType.NegativeBalanceAllowed != true && balance.RemainingDays < totalDays)
        {
            throw new BusinessException($"Insufficient leave balance. Remaining: {balance.RemainingDays}");
        }

        var leave = new LeaveRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            LeaveTypeId = dto.LeaveTypeId,
            FromDate = dto.FromDate,
            ToDate = dto.ToDate,
            Reason = dto.Reason,
            Status = "Pending",
            CreatedAt = DateTime.Now
        };

        await repository.AddLeaveAsync(leave, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Apply",
            nameof(LeaveRequest),
            leave.Id,
            $"Leave applied by employee {employeeId}",
            cancellationToken);

        logger.LogInformation("Leave {LeaveId} applied by employee {EmployeeId}", leave.Id, employeeId);
    }

    public async Task<List<LeaveBalanceDto>> GetMyLeaveBalancesAsync(
    CancellationToken cancellationToken = default)
    {
        var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
        if (employeeId == null)
        {
            throw new NotFoundException("Employee profile not found.");
        }

        var balances = await repository.GetEmployeeLeaveBalancesAsync(employeeId.Value, cancellationToken);

        return balances
            .Select(x => new LeaveBalanceDto
            {
                LeaveTypeId = x.LeaveTypeId ?? Guid.Empty,
                LeaveTypeName = x.LeaveType?.Name ?? string.Empty,
                AllocatedDays = x.AllocatedDays,
                UsedDays = x.UsedDays ?? 0,
                RemainingDays = x.RemainingDays
            })
            .ToList();
    }

    private static LeaveResponseDto MapToResponseDto(LeaveRequest leave)
    {
        return new LeaveResponseDto
        {
            Id = leave.Id,
            EmployeeName = $"{leave.Employee?.FirstName} {leave.Employee?.LastName}",
            FromDate = leave.FromDate,
            ToDate = leave.ToDate,
            Reason = leave.Reason,
            Status = leave.Status,
            ManagerComments = leave.ManagerComments,
            LeaveType = leave.LeaveType?.Name ?? string.Empty
        };
    }

    private static LeaveDetailsDto MapToDetailsDto(LeaveRequest leave)
    {
        return new LeaveDetailsDto
        {
            Id = leave.Id,
            EmployeeId = leave.EmployeeId ?? Guid.Empty,
            EmployeeName = $"{leave.Employee?.FirstName} {leave.Employee?.LastName}",
            LeaveTypeId = leave.LeaveTypeId ?? Guid.Empty,
            LeaveType = leave.LeaveType?.Name ?? string.Empty,
            FromDate = leave.FromDate,
            ToDate = leave.ToDate,
            TotalDays = leave.ToDate.DayNumber - leave.FromDate.DayNumber + 1,
            Reason = leave.Reason,
            Status = leave.Status,
            ManagerComments = leave.ManagerComments,
            CreatedAt = leave.CreatedAt
        };
    }

    public async Task<PagedResponse<LeaveResponseDto>> GetLeavesAsync(
    LeaveFilterDto filter,
    CancellationToken cancellationToken = default)
    {
        var query = repository.GetLeaves();

        if (!userContextService.IsAdminOrHr())
        {
            var role = userContextService.GetRole();
            var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);

            if (employeeId == null)
            {
                throw new NotFoundException("Employee profile not found.");
            }

            if (role == "Manager")
            {
                query = query.Where(x => x.Employee != null && x.Employee.ManagerId == employeeId.Value);
            }
            else
            {
                query = query.Where(x => x.EmployeeId == employeeId);
            }
        }

        query = ApplyFilters(query, filter);
        query = ApplySorting(query, filter);

        var totalRecords = await query.CountAsync(cancellationToken);

        var leaves = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<LeaveResponseDto>
        {
            Data = leaves.Select(MapToResponseDto),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize)
        };
    }

    public async Task<PagedResponse<LeaveResponseDto>> GetMyLeavesAsync(
    LeaveFilterDto filter,
    CancellationToken cancellationToken = default)
    {
        var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
        if (employeeId == null)
        {
            throw new NotFoundException("Employee profile not found.");
        }

        filter.EmployeeId = employeeId.Value;

        return await GetLeavesAsync(filter, cancellationToken);
    }

    public async Task<LeaveDetailsDto> GetLeaveAsync(
    Guid leaveId,
    CancellationToken cancellationToken = default)
    {
        var leave = await repository.GetLeaveByIdAsync(leaveId, cancellationToken);
        if (leave == null)
        {
            throw new NotFoundException("Leave request not found.");
        }

        if (!userContextService.IsAdminOrHr())
        {
            var role = userContextService.GetRole();
            var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);

            if (employeeId == null)
            {
                throw new NotFoundException("Employee profile not found.");
            }

            if (role == "Manager")
            {
                var isManager = await repository.IsManagerOfEmployeeAsync(
                    employeeId.Value,
                    leave.EmployeeId!.Value,
                    cancellationToken);

                if (!isManager)
                {
                    throw new BusinessException("Access denied.");
                }
            }
            else
            {
                if (leave.EmployeeId != employeeId)
                {
                    throw new BusinessException("Access denied.");
                }
            }
        }

        return MapToDetailsDto(leave);
    }

    private static IQueryable<LeaveRequest> ApplyFilters(
    IQueryable<LeaveRequest> query,
    LeaveFilterDto filter)
    {
        if (filter.EmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == filter.EmployeeId);
        }

        if (filter.LeaveTypeId.HasValue)
        {
            query = query.Where(x => x.LeaveTypeId == filter.LeaveTypeId);
        }

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            query = query.Where(x => x.Status == filter.Status);
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(x => x.FromDate >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(x => x.ToDate <= filter.ToDate.Value);
        }

        return query;
    }

    private static IQueryable<LeaveRequest> ApplySorting(
    IQueryable<LeaveRequest> query,
    LeaveFilterDto filter)
    {
        return filter.SortBy?.ToLower() switch
        {
            "fromdate" => filter.Descending
                ? query.OrderByDescending(x => x.FromDate)
                : query.OrderBy(x => x.FromDate),

            "todate" => filter.Descending
                ? query.OrderByDescending(x => x.ToDate)
                : query.OrderBy(x => x.ToDate),

            "status" => filter.Descending
                ? query.OrderByDescending(x => x.Status)
                : query.OrderBy(x => x.Status),

            _ => query.OrderByDescending(x => x.CreatedAt)
        };
    }

    public async Task ApproveLeaveAsync(
    Guid leaveId,
    LeaveActionDto dto,
    CancellationToken cancellationToken = default)
    {
        var leave = await repository.GetLeaveByIdAsync(leaveId, cancellationToken);
        if (leave == null)
        {
            throw new NotFoundException("Leave request not found.");
        }

        if (leave.Status != "Pending")
        {
            throw new BusinessException("Leave request already processed.");
        }

        if (!userContextService.IsAdminOrHr())
        {
            var role = userContextService.GetRole();
            if (role != "Manager")
            {
                throw new BusinessException("Access denied.");
            }

            var managerEmployeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
            if (managerEmployeeId == null)
            {
                throw new NotFoundException("Manager profile not found.");
            }

            var isManager = await repository.IsManagerOfEmployeeAsync(
                managerEmployeeId.Value,
                leave.EmployeeId ?? Guid.Empty,
                cancellationToken);

            if (!isManager)
            {
                throw new BusinessException("You can only approve leave for your team members.");
            }
        }

        var balance = await repository.GetLeaveBalanceAsync(
            leave.EmployeeId ?? Guid.Empty,
            leave.LeaveTypeId ?? Guid.Empty,
            cancellationToken);

        if (balance == null)
        {
            throw new NotFoundException("Leave balance not found.");
        }

        var leaveType = await repository.GetLeaveTypeAsync(
            leave.LeaveTypeId ?? Guid.Empty,
            cancellationToken);

        if (leaveType == null)
        {
            throw new NotFoundException("Leave type not found.");
        }

        var totalDays = leave.ToDate.DayNumber - leave.FromDate.DayNumber + 1;

        if (leaveType.NegativeBalanceAllowed != true && balance.RemainingDays < totalDays)
        {
            throw new BusinessException("Insufficient leave balance.");
        }

        balance.UsedDays = (balance.UsedDays ?? 0) + totalDays;
        balance.RemainingDays -= totalDays;

        leave.Status = "Approved";
        leave.ManagerComments = dto.ManagerComments;

        repository.UpdateLeaveBalance(balance);
        repository.UpdateLeave(leave);

        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Approve",
            nameof(LeaveRequest),
            leave.Id,
            $"Leave approved for employee {leave.EmployeeId}",
            cancellationToken);

        logger.LogInformation("Leave {LeaveId} approved", leave.Id);

        if (leave.Employee?.UserId != null)
        {
            notificationService.CreateNotification(
                leave.Employee.UserId.Value,
                "Leave Approved",
                "Your leave request has been approved.");
        }
    }

    public async Task RejectLeaveAsync(
    Guid leaveId,
    LeaveActionDto dto,
    CancellationToken cancellationToken = default)
    {
        var leave = await repository.GetLeaveByIdAsync(leaveId, cancellationToken);
        if (leave == null)
        {
            throw new NotFoundException("Leave request not found.");
        }

        if (leave.Status != "Pending")
        {
            throw new BusinessException("Leave request already processed.");
        }

        if (!userContextService.IsAdminOrHr())
        {
            var role = userContextService.GetRole();
            if (role != "Manager")
            {
                throw new BusinessException("Access denied.");
            }

            var managerEmployeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
            if (managerEmployeeId == null)
            {
                throw new NotFoundException("Manager profile not found.");
            }

            var isManager = await repository.IsManagerOfEmployeeAsync(
                managerEmployeeId.Value,
                leave.EmployeeId ?? Guid.Empty,
                cancellationToken);

            if (!isManager)
            {
                throw new BusinessException("You can only reject leave for your team members.");
            }
        }

        leave.Status = "Rejected";
        leave.ManagerComments = dto.ManagerComments;

        repository.UpdateLeave(leave);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Reject",
            nameof(LeaveRequest),
            leave.Id,
            $"Leave rejected for employee {leave.EmployeeId}",
            cancellationToken);

        logger.LogInformation("Leave {LeaveId} rejected", leave.Id);

        if (leave.Employee?.UserId != null)
        {
            notificationService.CreateNotification(
                leave.Employee.UserId.Value,
                "Leave Rejected",
                "Your leave request has been rejected.");
        }
    }

    public async Task WithdrawLeaveAsync(
    Guid leaveId,
    CancellationToken cancellationToken = default)
    {
        var leave = await repository.GetLeaveByIdAsync(leaveId, cancellationToken);
        if (leave == null)
        {
            throw new NotFoundException("Leave request not found.");
        }

        var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
        if (employeeId == null)
        {
            throw new NotFoundException("Employee profile not found.");
        }

        if (leave.EmployeeId != employeeId)
        {
            throw new BusinessException("You can only withdraw your own leave.");
        }

        if (leave.Status != "Pending")
        {
            throw new BusinessException("Only pending leave can be withdrawn.");
        }

        leave.Status = "Withdrawn";

        repository.UpdateLeave(leave);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Withdraw",
            nameof(LeaveRequest),
            leave.Id,
            $"Leave withdrawn by employee {employeeId}",
            cancellationToken);
    }

    public async Task CancelLeaveAsync(
    Guid leaveId,
    CancellationToken cancellationToken = default)
    {
        if (!userContextService.IsAdminOrHr())
        {
            throw new BusinessException("Access denied.");
        }

        var leave = await repository.GetLeaveByIdAsync(leaveId, cancellationToken);
        if (leave == null)
        {
            throw new NotFoundException("Leave request not found.");
        }

        if (leave.Status != "Approved")
        {
            throw new BusinessException("Only approved leave can be cancelled.");
        }

        var balance = await repository.GetLeaveBalanceAsync(
            leave.EmployeeId ?? Guid.Empty,
            leave.LeaveTypeId ?? Guid.Empty,
            cancellationToken);

        if (balance != null)
        {
            var totalDays = leave.ToDate.DayNumber - leave.FromDate.DayNumber + 1;

            balance.UsedDays = Math.Max((balance.UsedDays ?? 0) - totalDays, 0);
            balance.RemainingDays += totalDays;

            repository.UpdateLeaveBalance(balance);
        }

        leave.Status = "Cancelled";

        repository.UpdateLeave(leave);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Cancel",
            nameof(LeaveRequest),
            leave.Id,
            $"Leave cancelled for employee {leave.EmployeeId}",
            cancellationToken);

        logger.LogInformation("Leave {LeaveId} cancelled", leave.Id);

        if (leave.Employee?.UserId != null)
        {
            notificationService.CreateNotification(
                leave.Employee.UserId.Value,
                "Leave Cancelled",
                "Your approved leave has been cancelled.");
        }
    }
}
// using HRMS.API.Interfaces;
// using HRMS.API.Models.DTOs.Leave;
// using HRMS.API.Models.Entities;
// using HRMS.API.Exceptions;


// namespace HRMS.API.Services;

// public class LeaveService : ILeaveService
// {
//     private readonly ILeaveRepository leaveRepository;
//     private readonly INotificationService notificationService;

//     public LeaveService(ILeaveRepository leaveRepository , INotificationService notificationService)
//     {
//         this.leaveRepository = leaveRepository;
//         this.notificationService = notificationService;
//     }

//     public void ApplyLeave(ApplyLeaveDto dto,Guid userId,string role)
//     {


//         Guid employeeId;

//         if(role == "Employee" || role == "Manager")
//         {
//             var loggedInEmployee = leaveRepository.GetEmployeeByUserId(userId);

//             if(loggedInEmployee == null)
//             {
//                 throw new NotFoundException("Employee not found");
//             }

//             employeeId = loggedInEmployee.Id;
//         }
//         else
//         {
//             if(dto.EmployeeId == null)
//             {
//                 throw new BusinessException("employeeId is required");
//             }

//             employeeId = dto.EmployeeId.Value;
//         }
//         var employee = leaveRepository.GetEmployee(employeeId);

//         if(employee == null)
//         {
//                 throw new NotFoundException("Employee not found");
//         }

//         var leaveType = leaveRepository.GetLeaveType(dto.LeaveTypeId);

//         if(leaveType == null)
//         {
//             throw new NotFoundException("Leave type not found");
//         }

//         var balance = leaveRepository.GetLeaveBalance(employeeId,dto.LeaveTypeId);

//         if(balance == null)
//         {
//             throw new NotFoundException("Leave balance not allocated");
//         }

//         int days = dto.ToDate.DayNumber - dto.FromDate.DayNumber + 1;

//         if(balance.RemainingDays < days)
//         {
//             throw new BusinessException($"Insufficient leave balance. Remaining: {balance.RemainingDays}");
//         }

//         if(dto.FromDate > dto.ToDate)
//         {
//             throw new BusinessException("FromDate cannot be greater than ToDate");
//         }

//         if(dto.FromDate < DateOnly.FromDateTime(DateTime.UtcNow))
//         {
//             throw new BusinessException("Cannot apply leave for past dates");
//         }


//         if(leaveRepository.HasOverlappingLeave(employee.Id,dto.FromDate,dto.ToDate))
//             {
//                 throw new BusinessException("Leave already exists for selected dates");
//             }

//         LeaveRequest leave = new LeaveRequest
//             {
//                 Id = Guid.NewGuid(),

//                 EmployeeId = employeeId,

//                 LeaveTypeId = dto.LeaveTypeId,

//                 FromDate = dto.FromDate,

//                 ToDate = dto.ToDate,

//                 Reason = dto.Reason,

//                 Status = "Pending"
//             };

//         leaveRepository.AddLeave(leave);

//         leaveRepository.SaveChanges();
//     }

//     public List<LeaveResponseDto> GetAllLeaves()
//     {
//         return leaveRepository.GetAllLeaves().Select(l => new LeaveResponseDto
//             {
//                 Id = l.Id,

//                 EmployeeName =l.Employee!.FirstName + " " +l.Employee.LastName,

//                 FromDate = l.FromDate,

//                 ToDate = l.ToDate,

//                 Reason = l.Reason,

//                 Status = l.Status,

//                 ManagerComments = l.ManagerComments,

//                 LeaveType = l.LeaveType?.Name ?? ""
//             }).ToList();
//     }

//     public List<LeaveResponseDto> GetEmployeeLeaves(Guid employeeId)
//     {
//         return leaveRepository
//             .GetEmployeeLeaves(employeeId)
//             .Select(l => new LeaveResponseDto
//             {
//                 Id = l.Id,

//                 EmployeeName =
//                     l.Employee!.FirstName + " " +
//                     l.Employee.LastName,

//                 FromDate = l.FromDate,

//                 ToDate = l.ToDate,

//                 Reason = l.Reason,

//                 Status = l.Status,

//                 ManagerComments = l.ManagerComments,

//                 LeaveType =
//                     l.LeaveType?.Name ?? ""
//             })
//             .ToList();
//     }

//     public void ApproveLeave(Guid leaveId,Guid userId,string role,LeaveActionDto dto)
//     {
//         var leave = leaveRepository.GetLeaveById(leaveId);

//         if(role == "Manager")
//         {
//             var manager = leaveRepository.GetEmployeeByUserId(userId);

//             if(manager == null)
//             {
//                 throw new NotFoundException("Manager not found");
//             }

//             if(leave?.Employee?.ManagerId!= manager.Id)
//             {
//                 throw new BusinessException("You are not authorized to approve this leave");
//             }
//         }

//         if (leave == null)
//         {
//             throw new NotFoundException("Leave request not found");
//         }

//         if(leave.Status != "Pending")
//         {
//             throw new BusinessException("Leave request already processed");
//         }

//         leave.Status = "Approved";

//         if(leave.LeaveTypeId == null)
//         {
//             throw new NotFoundException("Leave type missing");
//         }

//         var balance =leaveRepository.GetLeaveBalance(leave.EmployeeId!.Value,leave.LeaveTypeId.Value);

//         if(balance == null)
//         {
//             throw new NotFoundException("Leave balance not found");
//         }

//         int days = leave.ToDate.DayNumber - leave.FromDate.DayNumber + 1;

//         balance.UsedDays += days;

//         balance.RemainingDays -= days;

//         leaveRepository.UpdateLeaveBalance(balance);

//         leave.ManagerComments = dto.ManagerComments;

//         leaveRepository.UpdateLeave(leave);

//         leaveRepository.SaveChanges();

//         if (leave.Employee?.UserId != null)
//         {
//             notificationService.CreateNotification(leave.Employee.UserId.Value,"Leave Approved","Your leave request has been approved.");
//         }
//     }

//     public void RejectLeave(Guid leaveId,Guid userId,string role,LeaveActionDto dto)
//     {
//         var leave = leaveRepository.GetLeaveById(leaveId);

//         if(role == "Manager")
//         {
//             var manager = leaveRepository.GetEmployeeByUserId(userId);

//             if(manager == null)
//             {
//                 throw new NotFoundException("Manager not found");
//             }

//             if(leave?.Employee?.ManagerId!= manager.Id)
//             {
//                 throw new BusinessException("You are not authorized to reject this leave");
//             }
//         }

//         if (leave == null)
//         {
//             throw new NotFoundException("Leave request not found");
//         }

//         if(leave.Status != "Pending")
//         {
//             throw new BusinessException("Leave request already processed");
//         }

//         leave.Status = "Rejected";

//         leave.ManagerComments = dto.ManagerComments;

//         leaveRepository.UpdateLeave(leave);

//         leaveRepository.SaveChanges();

//         if (leave.Employee?.UserId != null)
//         {
//             notificationService.CreateNotification(leave.Employee.UserId.Value,
//                 "Leave Rejected",
//                 "Your leave request has been rejected.");
//         }
//     }


//         public List<LeaveResponseDto> GetMyLeaves(Guid userId)
//         {
//             var employee = leaveRepository.GetEmployeeByUserId(userId);

//             if(employee == null)
//             {
//                 throw new NotFoundException("Employee not found");
//             }

//             return GetEmployeeLeaves(employee.Id);
//         }
// }


using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Leave;
using HRMS.API.Models.Entities;
using HRMS.API.Validators;

namespace HRMS.API.Services;

public class LeaveService : ILeaveService
{
    private readonly ILeaveRepository leaveRepository;
    private readonly INotificationService notificationService;
    private readonly LeaveValidator leaveValidator;

    public LeaveService(ILeaveRepository leaveRepository, INotificationService notificationService, LeaveValidator leaveValidator)
    {
        this.leaveRepository = leaveRepository;
        this.notificationService = notificationService;
        this.leaveValidator = leaveValidator;
    }

    public async Task ApplyLeaveAsync(ApplyLeaveDto dto, Guid userId, string role)
    {
        await leaveValidator.ValidateApplyLeaveAsync(dto);

        Guid employeeId;

        if (role == "Employee" || role == "Manager")
        {
            var loggedInEmployee = await leaveRepository.GetEmployeeByUserIdAsync(userId);

            if (loggedInEmployee == null)
            {
                throw new NotFoundException("Employee not found");
            }

            employeeId = loggedInEmployee.Id;
        }
        else
        {
            if (dto.EmployeeId == null)
            {
                throw new BusinessException("EmployeeId is required");
            }

            employeeId = dto.EmployeeId.Value;
        }

        var employee = await leaveRepository.GetEmployeeAsync(employeeId);

        if (employee == null)
        {
            throw new NotFoundException("Employee not found");
        }

        var leaveType = await leaveRepository.GetLeaveTypeAsync(dto.LeaveTypeId);

        if (leaveType == null)
        {
            throw new NotFoundException("Leave type not found");
        }

        var balance = await leaveRepository.GetLeaveBalanceAsync(employeeId, dto.LeaveTypeId);

        if (balance == null)
        {
            throw new NotFoundException("Leave balance not allocated");
        }

        int days = dto.ToDate.DayNumber - dto.FromDate.DayNumber + 1;

        if (balance.RemainingDays < days)
        {
            throw new BusinessException($"Insufficient leave balance. Remaining: {balance.RemainingDays}");
        }

        bool overlap = await leaveRepository.HasOverlappingLeaveAsync(employeeId, dto.FromDate, dto.ToDate);

        if (overlap)
        {
            throw new BusinessException("Leave already exists for selected dates");
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
            CreatedAt = DateTime.UtcNow
        };

        await leaveRepository.AddLeaveAsync(leave);
        await leaveRepository.SaveChangesAsync();
    }

    public async Task<List<LeaveResponseDto>> GetAllLeavesAsync()
    {
        var leaves = await leaveRepository.GetAllLeavesAsync();

        return leaves.Select(l => new LeaveResponseDto
        {
            Id = l.Id,
            EmployeeName = $"{l.Employee!.FirstName} {l.Employee.LastName}",
            FromDate = l.FromDate,
            ToDate = l.ToDate,
            Reason = l.Reason,
            Status = l.Status,
            ManagerComments = l.ManagerComments,
            LeaveType = l.LeaveType?.Name ?? string.Empty
        }).ToList();
    }

    public async Task<List<LeaveResponseDto>> GetEmployeeLeavesAsync(Guid employeeId)
    {
        var leaves = await leaveRepository.GetEmployeeLeavesAsync(employeeId);

        return leaves.Select(l => new LeaveResponseDto
        {
            Id = l.Id,
            EmployeeName = $"{l.Employee!.FirstName} {l.Employee.LastName}",
            FromDate = l.FromDate,
            ToDate = l.ToDate,
            Reason = l.Reason,
            Status = l.Status,
            ManagerComments = l.ManagerComments,
            LeaveType = l.LeaveType?.Name ?? string.Empty
        }).ToList();
    }

    public async Task<List<LeaveResponseDto>> GetMyLeavesAsync(Guid userId)
    {
        var employee = await leaveRepository.GetEmployeeByUserIdAsync(userId);

        if (employee == null)
        {
            throw new NotFoundException("Employee not found");
        }

        return await GetEmployeeLeavesAsync(employee.Id);
    }

    public async Task ApproveLeaveAsync(Guid leaveId, Guid userId, string role, LeaveActionDto dto)
    {
        var leave = await leaveRepository.GetLeaveByIdAsync(leaveId);

        if (leave == null)
        {
            throw new NotFoundException("Leave request not found");
        }

        if (role == "Manager")
        {
            var manager = await leaveRepository.GetEmployeeByUserIdAsync(userId);

            if (manager == null)
            {
                throw new NotFoundException("Manager not found");
            }

            if (leave.Employee?.ManagerId != manager.Id)
            {
                throw new BusinessException("You are not authorized to approve this leave");
            }
        }

        if (leave.Status != "Pending")
        {
            throw new BusinessException("Leave request already processed");
        }

        var balance = await leaveRepository.GetLeaveBalanceAsync(leave.EmployeeId!.Value, leave.LeaveTypeId!.Value);

        if (balance == null)
        {
            throw new NotFoundException("Leave balance not found");
        }

        var leaveType = await leaveRepository.GetLeaveTypeAsync(leave.LeaveTypeId.Value);

        if (leaveType == null)
        {
            throw new NotFoundException("Leave type not found");
        }

        int days = leave.ToDate.DayNumber - leave.FromDate.DayNumber + 1;

        if (balance.RemainingDays < days && leaveType.NegativeBalanceAllowed != true)
        {
            throw new BusinessException("Insufficient leave balance");
        }

        balance.UsedDays += days;
        balance.RemainingDays -= days;

        leave.Status = "Approved";
        leave.ManagerComments = dto.ManagerComments;

        leaveRepository.UpdateLeaveBalance(balance);
        leaveRepository.UpdateLeave(leave);

        await leaveRepository.SaveChangesAsync();

        if (leave.Employee?.UserId != null)
        {
            notificationService.CreateNotification(
                leave.Employee.UserId.Value,
                "Leave Approved",
                "Your leave request has been approved.");
        }
    }

    public async Task RejectLeaveAsync(Guid leaveId, Guid userId, string role, LeaveActionDto dto)
    {
        var leave = await leaveRepository.GetLeaveByIdAsync(leaveId);

        if (leave == null)
        {
            throw new NotFoundException("Leave request not found");
        }

        if (role == "Manager")
        {
            var manager = await leaveRepository.GetEmployeeByUserIdAsync(userId);

            if (manager == null)
            {
                throw new NotFoundException("Manager not found");
            }

            if (leave.Employee?.ManagerId != manager.Id)
            {
                throw new BusinessException("You are not authorized to reject this leave");
            }
        }

        if (leave.Status != "Pending")
        {
            throw new BusinessException("Leave request already processed");
        }

        leave.Status = "Rejected";
        leave.ManagerComments = dto.ManagerComments;

        leaveRepository.UpdateLeave(leave);

        await leaveRepository.SaveChangesAsync();

        if (leave.Employee?.UserId != null)
        {
            notificationService.CreateNotification(
                leave.Employee.UserId.Value,
                "Leave Rejected",
                "Your leave request has been rejected.");
        }
    }
}
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Leave;
using HRMS.API.Models.Entities;
using HRMS.API.Exceptions;


namespace HRMS.API.Services;

public class LeaveService : ILeaveService
{
    private readonly ILeaveRepository leaveRepository;
    private readonly INotificationService notificationService;

    public LeaveService(ILeaveRepository leaveRepository , INotificationService notificationService)
    {
        this.leaveRepository = leaveRepository;
        this.notificationService = notificationService;
    }

    public void ApplyLeave(ApplyLeaveDto dto,Guid userId,string role)
    {


        Guid employeeId;

        if(role == "Employee" || role == "Manager")
        {
            var loggedInEmployee = leaveRepository.GetEmployeeByUserId(userId);

            if(loggedInEmployee == null)
            {
                throw new NotFoundException("Employee not found");
            }

            employeeId = loggedInEmployee.Id;
        }
        else
        {
            if(dto.EmployeeId == null)
            {
                throw new BusinessException("employeeId is required");
            }

            employeeId = dto.EmployeeId.Value;
        }
        var employee = leaveRepository.GetEmployee(employeeId);

        if(employee == null)
        {
                throw new NotFoundException("Employee not found");
        }

        var leaveType = leaveRepository.GetLeaveType(dto.LeaveTypeId);

        if(leaveType == null)
        {
            throw new NotFoundException("Leave type not found");
        }

        var balance = leaveRepository.GetLeaveBalance(employeeId,dto.LeaveTypeId);

        if(balance == null)
        {
            throw new NotFoundException("Leave balance not allocated");
        }

        int days = dto.ToDate.DayNumber - dto.FromDate.DayNumber + 1;

        if(balance.RemainingDays < days)
        {
            throw new BusinessException($"Insufficient leave balance. Remaining: {balance.RemainingDays}");
        }

        if(dto.FromDate > dto.ToDate)
        {
            throw new BusinessException("FromDate cannot be greater than ToDate");
        }

        if(dto.FromDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new BusinessException("Cannot apply leave for past dates");
        }


        if(leaveRepository.HasOverlappingLeave(employee.Id,dto.FromDate,dto.ToDate))
            {
                throw new BusinessException("Leave already exists for selected dates");
            }

        LeaveRequest leave = new LeaveRequest
            {
                Id = Guid.NewGuid(),

                EmployeeId = employeeId,

                LeaveTypeId = dto.LeaveTypeId,

                FromDate = dto.FromDate,

                ToDate = dto.ToDate,

                Reason = dto.Reason,

                Status = "Pending"
            };

        leaveRepository.AddLeave(leave);

        leaveRepository.SaveChanges();
    }

    public List<LeaveResponseDto> GetAllLeaves()
    {
        return leaveRepository.GetAllLeaves().Select(l => new LeaveResponseDto
            {
                Id = l.Id,

                EmployeeName =l.Employee!.FirstName + " " +l.Employee.LastName,

                FromDate = l.FromDate,

                ToDate = l.ToDate,

                Reason = l.Reason,

                Status = l.Status,

                ManagerComments = l.ManagerComments,

                LeaveType = l.LeaveType?.Name ?? ""
            }).ToList();
    }

    public List<LeaveResponseDto> GetEmployeeLeaves(Guid employeeId)
    {
        return leaveRepository
            .GetEmployeeLeaves(employeeId)
            .Select(l => new LeaveResponseDto
            {
                Id = l.Id,

                EmployeeName =
                    l.Employee!.FirstName + " " +
                    l.Employee.LastName,

                FromDate = l.FromDate,

                ToDate = l.ToDate,

                Reason = l.Reason,

                Status = l.Status,

                ManagerComments = l.ManagerComments,

                LeaveType =
                    l.LeaveType?.Name ?? ""
            })
            .ToList();
    }

    public void ApproveLeave(Guid leaveId,Guid userId,string role,LeaveActionDto dto)
    {
        var leave = leaveRepository.GetLeaveById(leaveId);

        if(role == "Manager")
        {
            var manager = leaveRepository.GetEmployeeByUserId(userId);

            if(manager == null)
            {
                throw new NotFoundException("Manager not found");
            }

            if(leave?.Employee?.ManagerId!= manager.Id)
            {
                throw new BusinessException("You are not authorized to approve this leave");
            }
        }

        if (leave == null)
        {
            throw new NotFoundException("Leave request not found");
        }

        if(leave.Status != "Pending")
        {
            throw new BusinessException("Leave request already processed");
        }

        leave.Status = "Approved";

        if(leave.LeaveTypeId == null)
        {
            throw new NotFoundException("Leave type missing");
        }

        var balance =leaveRepository.GetLeaveBalance(leave.EmployeeId!.Value,leave.LeaveTypeId.Value);

        if(balance == null)
        {
            throw new NotFoundException("Leave balance not found");
        }

        int days = leave.ToDate.DayNumber - leave.FromDate.DayNumber + 1;

        balance.UsedDays += days;

        balance.RemainingDays -= days;

        leaveRepository.UpdateLeaveBalance(balance);

        leave.ManagerComments = dto.ManagerComments;

        leaveRepository.UpdateLeave(leave);

        leaveRepository.SaveChanges();

        if (leave.Employee?.UserId != null)
        {
            notificationService.CreateNotification(leave.Employee.UserId.Value,"Leave Approved","Your leave request has been approved.");
        }
    }

    public void RejectLeave(Guid leaveId,Guid userId,string role,LeaveActionDto dto)
    {
        var leave = leaveRepository.GetLeaveById(leaveId);

        if(role == "Manager")
        {
            var manager = leaveRepository.GetEmployeeByUserId(userId);

            if(manager == null)
            {
                throw new NotFoundException("Manager not found");
            }

            if(leave?.Employee?.ManagerId!= manager.Id)
            {
                throw new BusinessException("You are not authorized to reject this leave");
            }
        }

        if (leave == null)
        {
            throw new NotFoundException("Leave request not found");
        }

        if(leave.Status != "Pending")
        {
            throw new BusinessException("Leave request already processed");
        }

        leave.Status = "Rejected";

        leave.ManagerComments = dto.ManagerComments;

        leaveRepository.UpdateLeave(leave);

        leaveRepository.SaveChanges();

        if (leave.Employee?.UserId != null)
        {
            notificationService.CreateNotification(leave.Employee.UserId.Value,
                "Leave Rejected",
                "Your leave request has been rejected.");
        }
    }


        public List<LeaveResponseDto> GetMyLeaves(Guid userId)
        {
            var employee = leaveRepository.GetEmployeeByUserId(userId);

            if(employee == null)
            {
                throw new NotFoundException("Employee not found");
            }

            return GetEmployeeLeaves(employee.Id);
        }
}
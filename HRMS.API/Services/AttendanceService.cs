using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Attendance;
using HRMS.API.Models.DTOs.Common;
using HRMS.API.Models.Entities;
using HRMS.API.Validators;

namespace HRMS.API.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository attendanceRepository;
    private readonly AttendanceValidator attendanceValidator;

    public AttendanceService(IAttendanceRepository attendanceRepository, AttendanceValidator attendanceValidator)
    {
        this.attendanceRepository = attendanceRepository;
        this.attendanceValidator = attendanceValidator;
    }

    public async Task CheckInAsync(Guid userId)
    {
        var employee = await attendanceRepository.GetEmployeeByUserIdAsync(userId);

        if (employee == null)
        {
            throw new NotFoundException("Employee not found");
        }

        var alreadyCheckedIn = await attendanceRepository.GetTodayAttendanceAsync(employee.Id);

        if (alreadyCheckedIn != null)
        {
            throw new BusinessException("Already checked in today");
        }

        var attendance = new AttendanceLog
        {
            Id = Guid.NewGuid(),
            EmployeeId = employee.Id,
            AttendanceDate = DateOnly.FromDateTime(DateTime.Now),
            CheckIn = DateTime.Now,
            Status = "Present"
        };

        await attendanceRepository.AddAttendanceAsync(attendance);
        await attendanceRepository.SaveChangesAsync();
    }

    public async Task CheckOutAsync(Guid userId)
    {
        var employee = await attendanceRepository.GetEmployeeByUserIdAsync(userId);

        if (employee == null)
        {
            throw new NotFoundException("Employee not found");
        }

        var attendance = await attendanceRepository.GetTodayAttendanceAsync(employee.Id);

        if (attendance == null)
        {
            throw new NotFoundException("Check-in not found");
        }

        if (attendance.CheckOut != null)
        {
            throw new BusinessException("Already checked out");
        }

        attendance.CheckOut = DateTime.Now;

        attendanceRepository.UpdateAttendance(attendance);
        await attendanceRepository.SaveChangesAsync();
    }

    public async Task<PaginatedResponseDto<AttendanceResponseDto>> GetAttendanceAsync(AttendanceQueryDto query)
    {
        await attendanceValidator.ValidateQueryAsync(query);

        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var attendance = await attendanceRepository.GetAttendanceAsync(query, skip, pageSize);
        var totalRecords = await attendanceRepository.GetAttendanceCountAsync(query);

        var data = attendance.Select(a => new AttendanceResponseDto
        {
            Id = a.Id,
            EmployeeName = a.Employee!.FirstName + " " + a.Employee.LastName,
            AttendanceDate = a.AttendanceDate,
            CheckIn = a.CheckIn,
            CheckOut = a.CheckOut,
            Status = a.Status
        }).ToList();

        return new PaginatedResponseDto<AttendanceResponseDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            Data = data
        };
    }

    public async Task<List<AttendanceResponseDto>> GetEmployeeAttendanceAsync(Guid employeeId)
    {
        var employee = await attendanceRepository.GetEmployeeAsync(employeeId);

        if (employee == null)
        {
            throw new NotFoundException("Employee not found");
        }

        var attendance = await attendanceRepository.GetEmployeeAttendanceAsync(employeeId);

        return attendance.Select(a => new AttendanceResponseDto
        {
            Id = a.Id,
            EmployeeName = a.Employee!.FirstName + " " + a.Employee.LastName,
            AttendanceDate = a.AttendanceDate,
            CheckIn = a.CheckIn,
            CheckOut = a.CheckOut,
            Status = a.Status
        }).ToList();
    }
}
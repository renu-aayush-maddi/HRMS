using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Attendance;
using HRMS.API.Models.Entities;
using HRMS.API.Exceptions;
using HRMS.API.Models.DTOs.Common;

namespace HRMS.API.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository attendanceRepository;

    public AttendanceService(IAttendanceRepository attendanceRepository)
    {
        this.attendanceRepository = attendanceRepository;
    }
    public void CheckIn(Guid userId)
    {
        var employee = attendanceRepository.GetEmployeeByUserId(userId);

        if (employee == null)
        {
            throw new NotFoundException("Employee not found");
        }

        var alreadyCheckedIn =attendanceRepository.GetTodayAttendance(employee.Id);

        if (alreadyCheckedIn != null)
        {
            throw new BusinessException("Already checked in today");
        }

        AttendanceLog attendance = new AttendanceLog
            {
                Id = Guid.NewGuid(),

                EmployeeId = employee.Id,

                AttendanceDate = DateOnly.FromDateTime(DateTime.Now),

                CheckIn = DateTime.Now,

                Status = "Present"
            };

        attendanceRepository.AddAttendance(attendance);

        attendanceRepository.SaveChanges();
    }


    public void CheckOut(Guid userId)
    {
        var employee =attendanceRepository.GetEmployeeByUserId(userId);

        if (employee == null)
        {
            throw new NotFoundException("Employee not found");
        }

        var attendance = attendanceRepository.GetTodayAttendance(employee.Id);

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

        attendanceRepository.SaveChanges();
    }

    public PaginatedResponseDto<AttendanceResponseDto> GetAttendance(AttendanceQueryDto query)
    {
        var page =
            query.Page <= 0
                ? 1
                : query.Page;

        var pageSize =
            query.PageSize <= 0
                ? 10
                : query.PageSize;

        var skip =
            (page - 1) * pageSize;

        var attendance =
            attendanceRepository
            .GetAttendance(
                query,
                skip,
                pageSize);

        var totalRecords =
            attendanceRepository
            .GetAttendanceCount(
                query);

        var data =
            attendance
            .Select(a =>
                new AttendanceResponseDto
                {
                    Id = a.Id,

                    EmployeeName =
                        a.Employee!.FirstName +
                        " " +
                        a.Employee.LastName,

                    AttendanceDate =
                        a.AttendanceDate,

                    CheckIn =
                        a.CheckIn,

                    CheckOut =
                        a.CheckOut,

                    Status =
                        a.Status
                })
            .ToList();

        return new PaginatedResponseDto
            <AttendanceResponseDto>
        {
            Page = page,

            PageSize = pageSize,

            TotalRecords =
                totalRecords,

            Data = data
        };
    }

    public List<AttendanceResponseDto> GetEmployeeAttendance(Guid employeeId)
    {
        return attendanceRepository
            .GetEmployeeAttendance(employeeId)
            .Select(a => new AttendanceResponseDto
            {
                Id = a.Id,

                EmployeeName =
                    a.Employee!.FirstName + " " +
                    a.Employee.LastName,

                AttendanceDate = a.AttendanceDate,

                CheckIn = a.CheckIn,

                CheckOut = a.CheckOut,

                Status = a.Status
            })
            .ToList();
    }
}
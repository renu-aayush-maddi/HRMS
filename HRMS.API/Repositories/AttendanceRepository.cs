using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using HRMS.API.Models.DTOs.Attendance;

namespace HRMS.API.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly AppDbContext context;

    public AttendanceRepository(AppDbContext context)
    {
        this.context = context;
    }

    public AttendanceLog? GetTodayAttendance(Guid employeeId)
    {
        return context.AttendanceLogs
            .FirstOrDefault(a =>
                a.EmployeeId == employeeId
                &&
                a.AttendanceDate ==
                DateOnly.FromDateTime(DateTime.Now));
    }

    public List<AttendanceLog> GetAttendance(AttendanceQueryDto query,int skip,int take)
    {
        var attendance = context.AttendanceLogs.Include(a => a.Employee).AsQueryable();

        if (query.EmployeeId.HasValue)
        {
            attendance =attendance.Where(a => a.EmployeeId == query.EmployeeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            attendance = attendance.Where(a => a.Status == query.Status);
        }

        if (query.FromDate.HasValue)
        {
            attendance = attendance.Where(a => a.AttendanceDate >= query.FromDate.Value);
        }

        if (query.ToDate.HasValue)
        {
            attendance = attendance.Where(a => a.AttendanceDate <= query.ToDate.Value);
        }

        attendance = ApplySorting(attendance,query);

        return attendance.Skip(skip).Take(take).ToList();
    }

    public List<AttendanceLog> GetEmployeeAttendance(Guid employeeId)
    {
        return context.AttendanceLogs
            .Include(a => a.Employee)
            .Where(a => a.EmployeeId == employeeId)
            .ToList();
    }

    public void AddAttendance(AttendanceLog attendance)
    {
        context.AttendanceLogs.Add(attendance);
    }

    public void UpdateAttendance(AttendanceLog attendance)
    {
        context.AttendanceLogs.Update(attendance);
    }

    public Employee? GetEmployee(Guid employeeId)
    {
        return context.Employees.FirstOrDefault(e => e.Id == employeeId);
    }

    public Employee? GetEmployeeByUserId(Guid userId)
    {
        return context.Employees.FirstOrDefault(e => e.UserId == userId);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }

    public int GetAttendanceCount(AttendanceQueryDto query)
    {
        var attendance = context.AttendanceLogs.AsQueryable();

        if (query.EmployeeId.HasValue)
        {
            attendance =
                attendance.Where(a =>
                    a.EmployeeId ==
                    query.EmployeeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(
            query.Status))
        {
            attendance =
                attendance.Where(a =>
                    a.Status ==
                    query.Status);
        }

        if (query.FromDate.HasValue)
        {
            attendance =
                attendance.Where(a =>
                    a.AttendanceDate >=
                    query.FromDate.Value);
        }

        if (query.ToDate.HasValue)
        {
            attendance =
                attendance.Where(a =>
                    a.AttendanceDate <=
                    query.ToDate.Value);
        }

        return attendance.Count();
    }

    private IQueryable<AttendanceLog> ApplySorting( IQueryable<AttendanceLog> attendance, AttendanceQueryDto query)
    {
        var sortBy =
            query.SortBy?.ToLower();

        var direction =
            query.SortDirection?.ToLower();

        return (sortBy, direction) switch
        {
            ("attendancedate", "desc")
                => attendance.OrderByDescending(
                    a => a.AttendanceDate),

            ("attendancedate", _)
                => attendance.OrderBy(
                    a => a.AttendanceDate),

            ("checkin", "desc")
                => attendance.OrderByDescending(
                    a => a.CheckIn),

            ("checkin", _)
                => attendance.OrderBy(
                    a => a.CheckIn),

            ("status", "desc")
                => attendance.OrderByDescending(
                    a => a.Status),

            ("status", _)
                => attendance.OrderBy(
                    a => a.Status),

            _
                => attendance.OrderByDescending(
                    a => a.AttendanceDate)
        };
    }
}
using HRMS.API.Data;
using HRMS.API.Interfaces;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Repositories;

public class PayrollRepository : IPayrollRepository
{
    private readonly AppDbContext context;

    public PayrollRepository(AppDbContext context)
    {
        this.context = context;
    }

    public Employee? GetEmployee(Guid employeeId)
    {
        return context.Employees
            .FirstOrDefault(e => e.Id == employeeId);
    }

    public List<Payroll> GetAllPayrolls()
    {
        return context.Payrolls
            .Include(p => p.Employee)
            .ToList();
    }

    public List<Payroll> GetEmployeePayrolls(Guid employeeId)
    {
        return context.Payrolls
            .Include(p => p.Employee)
            .Where(p => p.EmployeeId == employeeId)
            .ToList();
    }

    public void AddPayroll(Payroll payroll)
    {
        context.Payrolls.Add(payroll);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }


    public int GetPresentDays(Guid employeeId,int month,int year)
    {
        return context.AttendanceLogs
            .Count(a =>
                a.EmployeeId == employeeId &&
                a.AttendanceDate.Month == month &&
                a.AttendanceDate.Year == year &&
                a.Status == "Present");
    }

    public int GetWorkingDays(
        int month,
        int year)
    {
        var holidays = context.Holidays
            .Where(h =>
                h.HolidayDate.Month == month &&
                h.HolidayDate.Year == year)
            .Select(h => h.HolidayDate)
            .ToHashSet();

        int workingDays = 0;

        int totalDays =
            DateTime.DaysInMonth(
                year,
                month);

        for (int day = 1; day <= totalDays; day++)
        {
            var date =
                new DateOnly(
                    year,
                    month,
                    day);

            bool isWeekend =
                date.DayOfWeek == DayOfWeek.Saturday ||
                date.DayOfWeek == DayOfWeek.Sunday;

            bool isHoliday =
                holidays.Contains(date);

            if (!isWeekend && !isHoliday)
            {
                workingDays++;
            }
        }

        return workingDays;
    }


    public int GetApprovedPaidLeaveDays(Guid employeeId,int month,int year)
    {
        return context.LeaveRequests
            .Include(x => x.LeaveType)
            .Where(l =>
                l.EmployeeId == employeeId &&
                l.Status == "Approved" &&
                l.LeaveType!.Name != "LOP")
            .AsEnumerable()
            .Sum(l =>
            {
                var start =
                    l.FromDate;

                var end =
                    l.ToDate;

                int days = 0;

                for (var date = start;
                    date <= end;
                    date = date.AddDays(1))
                {
                    if (date.Month == month &&
                        date.Year == year)
                    {
                        days++;
                    }
                }

                return days;
            });
    }


    public int GetApprovedLopLeaveDays(Guid employeeId,int month,int year)
    {
        return context.LeaveRequests
            .Include(x => x.LeaveType)
            .Where(l =>
                l.EmployeeId == employeeId &&
                l.Status == "Approved" &&
                l.LeaveType!.Name == "LOP")
            .AsEnumerable()
            .Sum(l =>
            {
                var start =
                    l.FromDate;

                var end =
                    l.ToDate;

                int days = 0;

                for (var date = start;
                    date <= end;
                    date = date.AddDays(1))
                {
                    if (date.Month == month &&
                        date.Year == year)
                    {
                        days++;
                    }
                }

                return days;
            });
    }

    public Payroll? GetPayrollById(Guid payrollId)
    {
        return context.Payrolls
            .Include(p => p.Employee)
            .ThenInclude(e => e.Department)
            .FirstOrDefault(p => p.Id == payrollId);
    }

    public void UpdatePayroll(Payroll payroll)
    {
        context.Payrolls.Update(payroll);
    }

    public Employee? GetEmployeeByUserId(Guid userId)
    {
        return context.Employees
            .FirstOrDefault(e =>
                e.UserId == userId);
    }


    public EmployeeSalary? GetActiveEmployeeSalary(
        Guid employeeId)
    {
        return context.EmployeeSalaries
            .Include(x => x.SalaryStructure)
            .FirstOrDefault(x =>
                x.EmployeeId == employeeId
                &&
                x.IsActive == true);
    }


    public decimal GetApprovedBonusAmount(Guid employeeId,int month,int year)
    {
        return context.Bonuses
            .Where(x =>
                x.EmployeeId == employeeId &&
                x.BonusMonth == month &&
                x.BonusYear == year &&
                x.Status == "Approved"
                && x.IsProcessed == false)
            .Sum(x => (decimal?)x.Amount) ?? 0;
    }


    public decimal GetApprovedDeductionAmount(Guid employeeId,int month,int year)
    {
        return context.Deductions
            .Where(x =>
                x.EmployeeId == employeeId &&
                x.DeductionMonth == month &&
                x.DeductionYear == year &&
                x.Status == "Approved"&&
                x.IsProcessed == false)
            .Sum(x => (decimal?)x.Amount) ?? 0;
    }


    public List<Employee> GetActiveEmployees()
    {
        return context.Employees
            .Where(x =>
                x.EmploymentStatus == "Active")
            .ToList();
    }

    public Payroll? GetPayroll(Guid employeeId,int month,int year)
    {
        return context.Payrolls
            .FirstOrDefault(x =>
                x.EmployeeId == employeeId &&
                x.PayMonth == month &&
                x.PayYear == year);
    }

    public List<Bonuse> GetApprovedBonuses(Guid employeeId,int month,int year)
    {
        return context.Bonuses
            .Where(x =>
                x.EmployeeId == employeeId &&
                x.BonusMonth == month &&
                x.BonusYear == year &&
                x.Status == "Approved" &&
                x.IsProcessed == false)
            .ToList();
    }

    public List<Deduction> GetApprovedDeductions(Guid employeeId,int month,int year)
    {
        return context.Deductions
            .Where(x =>
                x.EmployeeId == employeeId &&
                x.DeductionMonth == month &&
                x.DeductionYear == year &&
                x.Status == "Approved" &&
                x.IsProcessed == false)
            .ToList();
    }


}
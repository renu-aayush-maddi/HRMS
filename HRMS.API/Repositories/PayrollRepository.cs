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

    public async Task<Employee?> GetEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(x => x.Id == employeeId && !x.IsDeleted, cancellationToken);
    }

    public async Task<Employee?> GetEmployeeByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .FirstOrDefaultAsync(x => x.UserId == userId && !x.IsDeleted, cancellationToken);
    }

    public IQueryable<Payroll> GetPayrolls()
    {
        return context.Payrolls
            .AsNoTracking()
            .Include(x => x.Employee)
            .ThenInclude(x => x.Department);
    }

    public async Task<Payroll?> GetPayrollByIdAsync(Guid payrollId, CancellationToken cancellationToken = default)
    {
        return await context.Payrolls
            .Include(x => x.Employee)
            .ThenInclude(x => x.Department)
            .FirstOrDefaultAsync(x => x.Id == payrollId, cancellationToken);
    }

    public async Task<Payroll?> GetPayrollAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default)
    {
        return await context.Payrolls
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && 
                                      x.PayMonth == month && 
                                      x.PayYear == year, cancellationToken);
    }

    public async Task AddPayrollAsync(Payroll payroll, CancellationToken cancellationToken = default)
    {
        await context.Payrolls.AddAsync(payroll, cancellationToken);
    }

    public void UpdatePayroll(Payroll payroll)
    {
        context.Payrolls.Update(payroll);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetPresentDaysAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default)
    {
        return await context.AttendanceLogs
            .CountAsync(x => x.EmployeeId == employeeId && 
                             x.AttendanceDate.Month == month && 
                             x.AttendanceDate.Year == year && 
                             x.Status == "Present", cancellationToken);
    }

    public async Task<int> GetWorkingDaysAsync(int month, int year, CancellationToken cancellationToken = default)
    {
        var holidays = await context.Holidays
            .Where(x => x.HolidayDate.Month == month && x.HolidayDate.Year == year)
            .Select(x => x.HolidayDate)
            .ToListAsync(cancellationToken);

        var holidaySet = holidays.ToHashSet();
        var totalDays = DateTime.DaysInMonth(year, month);
        var workingDays = 0;

        for (var day = 1; day <= totalDays; day++)
        {
            var date = new DateOnly(year, month, day);
            var isWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
            var isHoliday = holidaySet.Contains(date);

            if (!isWeekend && !isHoliday)
            {
                workingDays++;
            }
        }

        return workingDays;
    }

    public async Task<int> GetApprovedPaidLeaveDaysAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default)
    {
        var leaves = await context.LeaveRequests
            .Include(x => x.LeaveType)
            .Where(x => x.EmployeeId == employeeId && 
                        x.Status == "Approved" && 
                        x.LeaveType != null && 
                        x.LeaveType.Name != "LOP")
            .ToListAsync(cancellationToken);

        return leaves.Sum(leave =>
        {
            var days = 0;
            for (var date = leave.FromDate; date <= leave.ToDate; date = date.AddDays(1))
            {
                if (date.Month == month && date.Year == year)
                {
                    days++;
                }
            }
            return days;
        });
    }

    public async Task<int> GetApprovedLopLeaveDaysAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default)
    {
        var leaves = await context.LeaveRequests
            .Include(x => x.LeaveType)
            .Where(x => x.EmployeeId == employeeId && 
                        x.Status == "Approved" && 
                        x.LeaveType != null && 
                        x.LeaveType.Name == "LOP")
            .ToListAsync(cancellationToken);

        return leaves.Sum(leave =>
        {
            var days = 0;
            for (var date = leave.FromDate; date <= leave.ToDate; date = date.AddDays(1))
            {
                if (date.Month == month && date.Year == year)
                {
                    days++;
                }
            }
            return days;
        });
    }

    public async Task<EmployeeSalary?> GetActiveEmployeeSalaryAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await context.EmployeeSalaries
            .Include(x => x.SalaryStructure)
            .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && x.IsActive == true, cancellationToken);
    }

    public async Task<decimal> GetApprovedBonusAmountAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default)
    {
        return await context.Bonuses
            .Where(x => x.EmployeeId == employeeId && 
                        x.BonusMonth == month && 
                        x.BonusYear == year && 
                        x.Status == "Approved" && 
                        x.IsProcessed == false)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0;
    }

    public async Task<decimal> GetApprovedDeductionAmountAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default)
    {
        return await context.Deductions
            .Where(x => x.EmployeeId == employeeId && 
                        x.DeductionMonth == month && 
                        x.DeductionYear == year && 
                        x.Status == "Approved" && 
                        x.IsProcessed == false)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0;
    }

    public async Task<List<Employee>> GetActiveEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return await context.Employees
            .Where(x => !x.IsDeleted && x.EmploymentStatus == "Active")
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Bonuse>> GetApprovedBonusesAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default)
    {
        return await context.Bonuses
            .Where(x => x.EmployeeId == employeeId && 
                        x.BonusMonth == month && 
                        x.BonusYear == year && 
                        x.Status == "Approved" && 
                        x.IsProcessed == false)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Deduction>> GetApprovedDeductionsAsync(Guid employeeId, int month, int year, CancellationToken cancellationToken = default)
    {
        return await context.Deductions
            .Where(x => x.EmployeeId == employeeId && 
                        x.DeductionMonth == month && 
                        x.DeductionYear == year && 
                        x.Status == "Approved" && 
                        x.IsProcessed == false)
            .ToListAsync(cancellationToken);
    }
}
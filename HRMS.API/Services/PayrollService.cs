using ClosedXML.Excel;
using HRMS.API.Common.Constants;
using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.Payroll;
using HRMS.API.Models.Entities;
using Microsoft.EntityFrameworkCore;


namespace HRMS.API.Services;

public class PayrollService : IPayrollService
{
    private readonly IPayrollRepository repository;
    private readonly IUserContextService userContextService;
    private readonly IAuditLogService auditLogService;
    private readonly INotificationService notificationService;
    private readonly ILogger<PayrollService> logger;

    public PayrollService(
        IPayrollRepository repository,
        IUserContextService userContextService,
        IAuditLogService auditLogService,
        INotificationService notificationService,
        ILogger<PayrollService> logger)
    {
        this.repository = repository;
        this.userContextService = userContextService;
        this.auditLogService = auditLogService;
        this.notificationService = notificationService;
        this.logger = logger;
    }


    public async Task<PayrollDetailDto> GeneratePayrollAsync(
        GeneratePayrollDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.PayMonth < 1 || dto.PayMonth > 12)
        {
            throw new BusinessException("Invalid payroll month.");
        }

        if (dto.PayYear < 2020)
        {
            throw new BusinessException("Invalid payroll year.");
        }

        var payrollDate = new DateOnly(dto.PayYear, dto.PayMonth, 1);
        var currentMonth = new DateOnly(DateTime.UtcNow.Year, DateTime.Now.Month, 1);

        if (payrollDate > currentMonth)
        {
            throw new BusinessException("Future payroll cannot be generated.");
        }

        var existingPayroll = await repository.GetPayrollAsync(dto.EmployeeId, dto.PayMonth, dto.PayYear, cancellationToken);
        if (existingPayroll != null)
        {
            throw new BusinessException("Payroll already generated.");
        }

        var employee = await repository.GetEmployeeAsync(dto.EmployeeId, cancellationToken);
        if (employee == null)
        {
            throw new NotFoundException("Employee not found.");
        }

        if (!string.Equals(employee.EmploymentStatus, "Active", StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessException("Payroll can only be generated for active employees.");
        }

        var employeeSalary = await repository.GetActiveEmployeeSalaryAsync(dto.EmployeeId, cancellationToken);
        if (employeeSalary == null)
        {
            throw new BusinessException("Employee salary not assigned.");
        }

        if (employeeSalary.EffectiveFrom > payrollDate)
        {
            throw new BusinessException("Salary structure is not effective for the selected payroll period.");
        }

        if (employee.JoiningDate > payrollDate)
        {
            throw new BusinessException("Payroll cannot be generated before employee joining date.");
        }

        decimal annualCtc = employeeSalary.AnnualCtc;
        decimal monthlySalary = annualCtc / 12;

        var structure = employeeSalary.SalaryStructure;

        decimal basicComponent = monthlySalary * structure.BasicPercentage / 100;
        decimal hraComponent = monthlySalary * structure.HraPercentage / 100;
        decimal specialAllowanceComponent = monthlySalary * structure.SpecialAllowancePercentage / 100;
        decimal medicalAllowanceComponent = monthlySalary * structure.MedicalAllowancePercentage / 100;
        decimal travelAllowanceComponent = monthlySalary * structure.TravelAllowancePercentage / 100;

        int workingDays = await repository.GetWorkingDaysAsync(dto.PayMonth, dto.PayYear, cancellationToken);
        int presentDays = await repository.GetPresentDaysAsync(dto.EmployeeId, dto.PayMonth, dto.PayYear, cancellationToken);
        int approvedPaidLeaveDays = await repository.GetApprovedPaidLeaveDaysAsync(dto.EmployeeId, dto.PayMonth, dto.PayYear, cancellationToken);
        int approvedLopLeaveDays = await repository.GetApprovedLopLeaveDaysAsync(dto.EmployeeId, dto.PayMonth, dto.PayYear, cancellationToken);

        int lopDays = Math.Max(workingDays - presentDays - approvedPaidLeaveDays, 0);

        decimal perDaySalary = workingDays == 0 ? 0 : monthlySalary / workingDays;
        decimal lopDeduction = perDaySalary * lopDays;

        decimal approvedBonus = await repository.GetApprovedBonusAmountAsync(dto.EmployeeId, dto.PayMonth, dto.PayYear, cancellationToken);
        decimal approvedDeduction = await repository.GetApprovedDeductionAmountAsync(dto.EmployeeId, dto.PayMonth, dto.PayYear, cancellationToken);

        decimal totalDeductions = approvedDeduction + lopDeduction;
        decimal netSalary = monthlySalary + approvedBonus - totalDeductions;

        var payroll = new Payroll
        {
            Id = Guid.NewGuid(),
            EmployeeId = dto.EmployeeId,
            PayMonth = dto.PayMonth,
            PayYear = dto.PayYear,
            BasicSalary = monthlySalary,
            BasicComponent = basicComponent,
            HraComponent = hraComponent,
            SpecialAllowanceComponent = specialAllowanceComponent,
            MedicalAllowanceComponent = medicalAllowanceComponent,
            TravelAllowanceComponent = travelAllowanceComponent,
            Bonus = approvedBonus,
            Deductions = totalDeductions,
            NetSalary = netSalary,
            WorkingDays = workingDays,
            PresentDays = presentDays,
            LopDays = lopDays,
            LopDeduction = lopDeduction,
            GeneratedAt = DateTime.UtcNow,
            Status = PayrollStatuses.Generated
        };

        await repository.AddPayrollAsync(payroll, cancellationToken);

        var bonuses = await repository.GetApprovedBonusesAsync(dto.EmployeeId, dto.PayMonth, dto.PayYear, cancellationToken);
        foreach (var bonus in bonuses)
        {
            bonus.IsProcessed = true;
        }

        var deductions = await repository.GetApprovedDeductionsAsync(dto.EmployeeId, dto.PayMonth, dto.PayYear, cancellationToken);
        foreach (var deduction in deductions)
        {
            deduction.IsProcessed = true;
        }

        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Generate",
            nameof(Payroll),
            payroll.Id,
            $"Payroll generated for employee {employee.Id}",
            cancellationToken);

        logger.LogInformation("Payroll {PayrollId} generated for employee {EmployeeId}", payroll.Id, employee.Id);

        if (employee.UserId.HasValue)
        {
            notificationService.CreateNotification(
                employee.UserId.Value,
                "Payroll Generated",
                $"Payroll generated for {dto.PayMonth}/{dto.PayYear}");
        }

        return MapToDetailDto(payroll, employee);
    }


    public async Task<PagedResponse<PayrollResponseDto>> GetPayrollsAsync(
    PayrollFilterDto filter,
    CancellationToken cancellationToken = default)
    {
        var query = repository.GetPayrolls();

        if (!userContextService.IsAdminOrHr())
        {
            var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
            if (employeeId == null)
            {
                throw new NotFoundException("Employee profile not found.");
            }

            query = query.Where(x => x.EmployeeId == employeeId);
        }

        query = ApplyFilters(query, filter);
        query = ApplySorting(query, filter);

        var totalRecords = await query.CountAsync(cancellationToken);

        var payrolls = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<PayrollResponseDto>
        {
            Data = payrolls.Select(MapToResponseDto),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)filter.PageSize)
        };
    }


    public async Task<PagedResponse<PayrollResponseDto>> GetEmployeePayrollsAsync(
    Guid employeeId,
    PayrollFilterDto filter,
    CancellationToken cancellationToken = default)
    {
        if (!userContextService.IsAdminOrHr())
        {
            throw new ForbiddenException("Only Admin and HR can access employee payrolls.");
        }

        filter.EmployeeId = employeeId;

        return await GetPayrollsAsync(filter, cancellationToken);
    }

    public async Task<PagedResponse<PayrollResponseDto>> GetMyPayrollsAsync(
    PayrollFilterDto filter,
    CancellationToken cancellationToken = default)
    {
        var employeeId =
            await userContextService.GetEmployeeIdAsync(
                cancellationToken);

        if (employeeId == null)
        {
            throw new NotFoundException(
                "Employee profile not found.");
        }

        filter.EmployeeId = employeeId;

        return await GetPayrollsAsync(
            filter,
            cancellationToken);
    }

    public async Task<PayrollDetailDto> GetPayrollAsync(
    Guid payrollId,
    CancellationToken cancellationToken = default)
    {
        var payroll = await repository.GetPayrollByIdAsync(payrollId, cancellationToken);
        if (payroll == null)
        {
            throw new NotFoundException("Payroll not found.");
        }

        if (!userContextService.IsAdminOrHr())
        {
            var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
            if (employeeId != payroll.EmployeeId)
            {
                throw new ForbiddenException("You can only access your own payroll.");
            }
        }

        return MapToDetailDto(payroll, payroll.Employee!);
    }

    private static IQueryable<Payroll> ApplyFilters(
    IQueryable<Payroll> query,
    PayrollFilterDto filter)
    {
        if (filter.EmployeeId.HasValue)
        {
            query = query.Where(x => x.EmployeeId == filter.EmployeeId);
        }

        if (filter.PayMonth.HasValue)
        {
            query = query.Where(x => x.PayMonth == filter.PayMonth);
        }

        if (filter.PayYear.HasValue)
        {
            query = query.Where(x => x.PayYear == filter.PayYear);
        }

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            query = query.Where(x => x.Status == filter.Status);
        }

        if (filter.MinNetSalary.HasValue)
        {
            query = query.Where(x => x.NetSalary >= filter.MinNetSalary);
        }

        if (filter.MaxNetSalary.HasValue)
        {
            query = query.Where(x => x.NetSalary <= filter.MaxNetSalary);
        }

        return query;
    }

    private static IQueryable<Payroll> ApplySorting(
    IQueryable<Payroll> query,
    PayrollFilterDto filter)
    {
        return filter.SortBy?.ToLower() switch
        {
            "netsalary" =>
                filter.Descending
                    ? query.OrderByDescending(x => x.NetSalary)
                    : query.OrderBy(x => x.NetSalary),

            "paymonth" =>
                filter.Descending
                    ? query.OrderByDescending(x => x.PayMonth)
                    : query.OrderBy(x => x.PayMonth),

            "payyear" =>
                filter.Descending
                    ? query.OrderByDescending(x => x.PayYear)
                    : query.OrderBy(x => x.PayYear),

            _ =>
                query.OrderByDescending(x => x.GeneratedAt)
        };
    }

    private static PayrollResponseDto MapToResponseDto(
    Payroll payroll)
    {
        return new PayrollResponseDto
        {
            Id = payroll.Id,

            EmployeeName =
                $"{payroll.Employee?.FirstName} {payroll.Employee?.LastName}",

            PayMonth = payroll.PayMonth,

            PayYear = payroll.PayYear,

            BasicSalary = payroll.BasicSalary,

            Bonus = payroll.Bonus,

            Deductions = payroll.Deductions,

            NetSalary = payroll.NetSalary,

            BasicComponent = payroll.BasicComponent,

            HraComponent = payroll.HraComponent,

            SpecialAllowanceComponent =
                payroll.SpecialAllowanceComponent,

            MedicalAllowanceComponent =
                payroll.MedicalAllowanceComponent,

            TravelAllowanceComponent =
                payroll.TravelAllowanceComponent,

            Status = payroll.Status ?? string.Empty,

            GeneratedAt = payroll.GeneratedAt
        };
    }

    private static PayrollDetailDto MapToDetailDto(Payroll payroll, Employee employee)
    {
        return new PayrollDetailDto
        {
            Id = payroll.Id,
            EmployeeId = employee.Id,
            EmployeeName = $"{employee.FirstName} {employee.LastName}",
            PayMonth = payroll.PayMonth,
            PayYear = payroll.PayYear,
            BasicSalary = payroll.BasicSalary,
            Bonus = payroll.Bonus,
            Deductions = payroll.Deductions,
            NetSalary = payroll.NetSalary,
            WorkingDays = payroll.WorkingDays,
            PresentDays = payroll.PresentDays,
            LopDays = payroll.LopDays,
            LopDeduction = payroll.LopDeduction,
            Status = payroll.Status ?? string.Empty,
            BasicComponent = payroll.BasicComponent,
            HraComponent = payroll.HraComponent,
            SpecialAllowanceComponent = payroll.SpecialAllowanceComponent,
            MedicalAllowanceComponent = payroll.MedicalAllowanceComponent,
            TravelAllowanceComponent = payroll.TravelAllowanceComponent,
            GeneratedAt = payroll.GeneratedAt
        };
    }


    public async Task ApprovePayrollAsync(
    Guid payrollId,
    CancellationToken cancellationToken = default)
    {
        var payroll = await repository.GetPayrollByIdAsync(payrollId, cancellationToken);
        if (payroll == null)
        {
            throw new NotFoundException("Payroll not found.");
        }

        if (payroll.Status != PayrollStatuses.Generated)
        {
            throw new BusinessException("Only generated payroll can be approved.");
        }

        payroll.Status = PayrollStatuses.Approved;

        repository.UpdatePayroll(payroll);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "Approve",
            nameof(Payroll),
            payroll.Id,
            "Payroll approved.",
            cancellationToken);

        logger.LogInformation("Payroll {PayrollId} approved", payroll.Id);

        if (payroll.Employee?.UserId != null)
        {
            notificationService.CreateNotification(
                payroll.Employee.UserId.Value,
                "Payroll Approved",
                $"Payroll for {payroll.PayMonth}/{payroll.PayYear} has been approved.");
        }
    }

    public async Task MarkPayrollPaidAsync(
    Guid payrollId,
    CancellationToken cancellationToken = default)
    {
        var payroll = await repository.GetPayrollByIdAsync(payrollId, cancellationToken);
        if (payroll == null)
        {
            throw new NotFoundException("Payroll not found.");
        }

        if (payroll.Status != PayrollStatuses.Approved)
        {
            throw new BusinessException("Only approved payroll can be marked as paid.");
        }

        payroll.Status = PayrollStatuses.Paid;

        repository.UpdatePayroll(payroll);
        await repository.SaveChangesAsync(cancellationToken);

        await auditLogService.LogAsync(
            "MarkPaid",
            nameof(Payroll),
            payroll.Id,
            "Payroll marked as paid.",
            cancellationToken);

        logger.LogInformation("Payroll {PayrollId} marked paid", payroll.Id);

        if (payroll.Employee?.UserId != null)
        {
            notificationService.CreateNotification(
                payroll.Employee.UserId.Value,
                "Salary Credited",
                $"Payroll for {payroll.PayMonth}/{payroll.PayYear} has been marked as paid.");
        }
    }

    public async Task<BulkPayrollResponseDto> GenerateMonthlyPayrollAsync(
    GenerateMonthlyPayrollDto dto,
    CancellationToken cancellationToken = default)
    {
        var employees = await repository.GetActiveEmployeesAsync(cancellationToken);

        var response = new BulkPayrollResponseDto
        {
            TotalEmployees = employees.Count
        };

        foreach (var employee in employees)
        {
            try
            {
                var existingPayroll = await repository.GetPayrollAsync(
                    employee.Id,
                    dto.PayMonth,
                    dto.PayYear,
                    cancellationToken);

                if (existingPayroll != null)
                {
                    response.SkippedCount++;
                    response.SkippedEmployees.Add($"{employee.FirstName} {employee.LastName}");
                    continue;
                }

                await GeneratePayrollAsync(
                    new GeneratePayrollDto
                    {
                        EmployeeId = employee.Id,
                        PayMonth = dto.PayMonth,
                        PayYear = dto.PayYear
                    },
                    cancellationToken);

                response.SuccessCount++;
            }
            catch (Exception ex)
            {
                response.FailedCount++;
                response.Errors.Add($"{employee.FirstName} {employee.LastName} : {ex.Message}");
            }
        }

        await auditLogService.LogAsync(
            "GenerateMonthly",
            nameof(Payroll),
            Guid.Empty,
            $"Monthly payroll generated for {dto.PayMonth}/{dto.PayYear}",
            cancellationToken);

        logger.LogInformation(
            "Monthly payroll generation completed for {Month}/{Year}",
            dto.PayMonth,
            dto.PayYear);

        return response;
    }

    public async Task<byte[]> ExportPayrollsAsync(
    PayrollFilterDto filter,
    CancellationToken cancellationToken = default)
    {
        var query = repository.GetPayrolls();

        if (!userContextService.IsAdminOrHr())
        {
            var employeeId = await userContextService.GetEmployeeIdAsync(cancellationToken);
            if (employeeId == null)
            {
                throw new NotFoundException("Employee profile not found.");
            }

            query = query.Where(x => x.EmployeeId == employeeId);
        }

        query = ApplyFilters(query, filter);

        var payrolls = await query.ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Payrolls");

        worksheet.Cell(1, 1).Value = "Employee";
        worksheet.Cell(1, 2).Value = "Month";
        worksheet.Cell(1, 3).Value = "Year";
        worksheet.Cell(1, 4).Value = "Basic Salary";
        worksheet.Cell(1, 5).Value = "Bonus";
        worksheet.Cell(1, 6).Value = "Deductions";
        worksheet.Cell(1, 7).Value = "Net Salary";
        worksheet.Cell(1, 8).Value = "Status";

        var row = 2;
        foreach (var payroll in payrolls)
        {
            worksheet.Cell(row, 1).Value = $"{payroll.Employee?.FirstName} {payroll.Employee?.LastName}";
            worksheet.Cell(row, 2).Value = payroll.PayMonth;
            worksheet.Cell(row, 3).Value = payroll.PayYear;
            worksheet.Cell(row, 4).Value = payroll.BasicSalary;
            worksheet.Cell(row, 5).Value = payroll.Bonus ?? 0;
            worksheet.Cell(row, 6).Value = payroll.Deductions ?? 0;
            worksheet.Cell(row, 7).Value = payroll.NetSalary ?? 0;
            worksheet.Cell(row, 8).Value = payroll.Status ?? string.Empty;
            row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        await auditLogService.LogAsync(
            "Export",
            nameof(Payroll),
            Guid.Empty,
            $"{payrolls.Count} payrolls exported.",
            cancellationToken);

        return stream.ToArray();
    }
}
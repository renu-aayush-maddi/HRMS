using HRMS.API.Interfaces;
using HRMS.API.Models.DTOs.Payroll;
using HRMS.API.Models.Entities;

namespace HRMS.API.Services;

    public class PayrollService : IPayrollService
    {
        private readonly IPayrollRepository payrollRepository;
        private readonly INotificationService notificationService;

        public PayrollService(IPayrollRepository payrollRepository, INotificationService notificationService)
        {
            this.payrollRepository = payrollRepository;
            this.notificationService = notificationService;
        }

    public void GeneratePayroll(GeneratePayrollDto dto)
    {

        var existingPayroll = payrollRepository.GetPayroll(dto.EmployeeId,dto.PayMonth,dto.PayYear);

        if (existingPayroll != null)
        {
            throw new Exception(
                "Payroll already generated");
        }
        var employee = payrollRepository.GetEmployee(dto.EmployeeId);

        if (employee == null)
        {
            throw new Exception("Employee not found");
        }

        var employeeSalary =
            payrollRepository
                .GetActiveEmployeeSalary(
                    dto.EmployeeId);

        if (employeeSalary == null)
        {
            throw new Exception(
                "Employee salary not assigned");
        }

        decimal annualCtc =
            employeeSalary.AnnualCtc;

        decimal monthlySalary =
            annualCtc / 12;

        var structure =
            employeeSalary.SalaryStructure;

        decimal basicComponent =
            monthlySalary *
            structure.BasicPercentage / 100;

        decimal hraComponent =
            monthlySalary *
            structure.HraPercentage / 100;

        decimal specialAllowanceComponent =
            monthlySalary *
            structure.SpecialAllowancePercentage / 100;

        decimal medicalAllowanceComponent =
            monthlySalary *
            structure.MedicalAllowancePercentage / 100;

        decimal travelAllowanceComponent =
            monthlySalary *
            structure.TravelAllowancePercentage / 100;

        int workingDays =
            payrollRepository.GetWorkingDays(
                dto.PayMonth,
                dto.PayYear);

        int presentDays =
            payrollRepository.GetPresentDays(
                dto.EmployeeId,
                dto.PayMonth,
                dto.PayYear);

        int approvedPaidLeaveDays =
            payrollRepository.GetApprovedPaidLeaveDays(
                dto.EmployeeId,
                dto.PayMonth,
                dto.PayYear);

        int approvedLopLeaveDays =
            payrollRepository.GetApprovedLopLeaveDays(
                dto.EmployeeId,
                dto.PayMonth,
                dto.PayYear);

        int lopDays =
            Math.Max(
                workingDays -
                presentDays -
                approvedPaidLeaveDays,
                0);

        lopDays += approvedLopLeaveDays;

        decimal perDaySalary =
            workingDays == 0
                ? 0
                : monthlySalary / workingDays;

        decimal lopDeduction =
            perDaySalary * lopDays;

        decimal approvedBonus =
            payrollRepository
                .GetApprovedBonusAmount(
                    dto.EmployeeId,
                    dto.PayMonth,
                    dto.PayYear);

        decimal approvedDeduction =
            payrollRepository
                .GetApprovedDeductionAmount(
                    dto.EmployeeId,
                    dto.PayMonth,
                    dto.PayYear);

        decimal totalDeductions =
            approvedDeduction +
            lopDeduction;

        decimal netSalary =
            monthlySalary +
            approvedBonus -
            totalDeductions;

        Payroll payroll = new Payroll
        {
            Id = Guid.NewGuid(),

            EmployeeId = dto.EmployeeId,

            PayMonth = dto.PayMonth,

            PayYear = dto.PayYear,

            BasicSalary = monthlySalary,

            BasicComponent =
                basicComponent,

            HraComponent =
                hraComponent,

            SpecialAllowanceComponent =
                specialAllowanceComponent,

            MedicalAllowanceComponent =
                medicalAllowanceComponent,

            TravelAllowanceComponent =
                travelAllowanceComponent,

            Bonus =
                approvedBonus,

            Deductions =
                totalDeductions,

            NetSalary =
                netSalary,

            WorkingDays =
                workingDays,

            PresentDays =
                presentDays,

            LopDays =
                lopDays,

            LopDeduction =
                lopDeduction,

            Status =
                "Generated"
        };

        payrollRepository.AddPayroll(payroll);

        var bonuses =payrollRepository.GetApprovedBonuses(
            dto.EmployeeId,
            dto.PayMonth,
            dto.PayYear);

        foreach (var bonus in bonuses)
        {
            bonus.IsProcessed = true;
        }

        var deductions = payrollRepository.GetApprovedDeductions(
                    dto.EmployeeId,
                    dto.PayMonth,
                    dto.PayYear);

        foreach (var deduction in deductions)
        {
            deduction.IsProcessed = true;
        }

        payrollRepository.SaveChanges();

        notificationService.CreateNotification(
            employee.UserId!.Value,
            "Payroll Generated",
            $"Payroll generated for {dto.PayMonth}/{dto.PayYear}");
    }


         

    public List<PayrollResponseDto> GetAllPayrolls()
    {
        return payrollRepository
            .GetAllPayrolls()
            .Select(p => new PayrollResponseDto
            {
                Id = p.Id,

                EmployeeName =
                    p.Employee!.FirstName + " " +
                    p.Employee.LastName,

                PayMonth = p.PayMonth,

                PayYear = p.PayYear,

                BasicSalary = p.BasicSalary,

                BasicComponent =
                    p.BasicComponent,

                HraComponent =
                    p.HraComponent,

                SpecialAllowanceComponent =
                    p.SpecialAllowanceComponent,

                MedicalAllowanceComponent =
                    p.MedicalAllowanceComponent,

                TravelAllowanceComponent =
                    p.TravelAllowanceComponent,

                Bonus = p.Bonus,

                Deductions = p.Deductions,

                NetSalary = p.NetSalary
            })
            .ToList();
    }

    public List<PayrollResponseDto>
        GetEmployeePayrolls(Guid employeeId)
    {
        return payrollRepository
            .GetEmployeePayrolls(employeeId)
            .Select(p => new PayrollResponseDto
            {
                Id = p.Id,

                EmployeeName =
                    p.Employee!.FirstName + " " +
                    p.Employee.LastName,

                PayMonth = p.PayMonth,

                PayYear = p.PayYear,

                BasicSalary = p.BasicSalary,

                BasicComponent =
                    p.BasicComponent,

                HraComponent =
                    p.HraComponent,

                SpecialAllowanceComponent =
                    p.SpecialAllowanceComponent,

                MedicalAllowanceComponent =
                    p.MedicalAllowanceComponent,

                TravelAllowanceComponent =
                    p.TravelAllowanceComponent,

                Bonus = p.Bonus,

                Deductions = p.Deductions,

                NetSalary = p.NetSalary
            })
            .ToList();
    }


    public void ApprovePayroll(Guid payrollId)
    {
        var payroll =
            payrollRepository
            .GetPayrollById(payrollId);

        if (payroll == null)
        {
            throw new Exception("Payroll not found");
        }

        if (payroll.Status != "Generated")
        {
            throw new Exception(
                "Only generated payroll can be approved");
        }

        payroll.Status = "Approved";

        payrollRepository.UpdatePayroll(payroll);

        payrollRepository.SaveChanges();

        if (payroll.Employee?.UserId != null)
        {
            notificationService.CreateNotification(
                payroll.Employee.UserId.Value,
                "Payroll Approved",
                $"Payroll for {payroll.PayMonth}/{payroll.PayYear} has been approved.");
        }
    }

    public void MarkPayrollPaid(Guid payrollId)
    {
        var payroll =
            payrollRepository
            .GetPayrollById(payrollId);

        if (payroll == null)
        {
            throw new Exception("Payroll not found");
        }

        if (payroll.Status != "Approved")
        {
            throw new Exception(
                "Only approved payroll can be marked as paid");
        }

        payroll.Status = "Paid";

        payrollRepository.UpdatePayroll(payroll);

        payrollRepository.SaveChanges();

        if (payroll.Employee?.UserId != null)
        {
            notificationService.CreateNotification(
                payroll.Employee.UserId.Value,
                "Salary Credited",
                $"Salary for {payroll.PayMonth}/{payroll.PayYear} has been credited.");
        }
    }


    public List<PayrollResponseDto> GetMyPayrolls(Guid userId)
    {
        var employee =
            payrollRepository
            .GetEmployeeByUserId(userId);

        if (employee == null)
        {
            throw new Exception(
                "Employee not found");
        }

        return payrollRepository
            .GetEmployeePayrolls(employee.Id)
            .Select(p => new PayrollResponseDto
            {
                Id = p.Id,

                EmployeeName =
                    p.Employee!.FirstName +
                    " " +
                    p.Employee.LastName,

                PayMonth = p.PayMonth,

                PayYear = p.PayYear,

                BasicSalary = p.BasicSalary,

                BasicComponent =
                    p.BasicComponent,

                HraComponent =
                    p.HraComponent,

                SpecialAllowanceComponent =
                    p.SpecialAllowanceComponent,

                MedicalAllowanceComponent =
                    p.MedicalAllowanceComponent,

                TravelAllowanceComponent =
                    p.TravelAllowanceComponent,

                Bonus = p.Bonus,

                Deductions = p.Deductions,

                NetSalary = p.NetSalary
            })
            .ToList();
    }



    public BulkPayrollResponseDto
    GenerateMonthlyPayroll(
        GenerateMonthlyPayrollDto dto)
{
    var employees =
        payrollRepository
            .GetActiveEmployees();

    BulkPayrollResponseDto result =
        new()
        {
            TotalEmployees =
                employees.Count
        };

    foreach (var employee in employees)
    {
        try
        {
            GeneratePayroll(
                new GeneratePayrollDto
                {
                    EmployeeId =
                        employee.Id,

                    PayMonth =
                        dto.PayMonth,

                    PayYear =
                        dto.PayYear
                });

            result.SuccessCount++;
        }
        catch (Exception ex)
        {
            result.FailedCount++;

            result.Errors.Add(
                $"{employee.EmployeeCode} : {ex.Message}");
        }
    }

    return result;
}
}
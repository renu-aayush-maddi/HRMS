using HRMS.API.Exceptions;
using HRMS.API.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace HRMS.API.Services;

public class PayslipService : IPayslipService
{
    private readonly IPayrollRepository payrollRepository;
    private readonly IUserContextService userContextService;

    public PayslipService(
        IPayrollRepository payrollRepository,
        IUserContextService userContextService)
    {
        this.payrollRepository = payrollRepository;
        this.userContextService = userContextService;
    }

    public async Task<byte[]> GeneratePayslipAsync(
    Guid payrollId,
    CancellationToken cancellationToken = default)
    {
        var payroll =
            await payrollRepository.GetPayrollByIdAsync(
                payrollId,
                cancellationToken);

        if (payroll == null)
        {
            throw new NotFoundException("Payroll not found");
        }

        return BuildPayslip(payroll);
    }

    public async Task<byte[]> GenerateMyPayslipAsync(
        Guid payrollId,
        CancellationToken cancellationToken = default)
    {
        var payroll =
            await payrollRepository.GetPayrollByIdAsync(
                payrollId,
                cancellationToken);

        if (payroll == null)
        {
            throw new NotFoundException(
                "Payroll not found");
        }

        var employeeId =
            await userContextService.GetEmployeeIdAsync(
                cancellationToken);

        if (employeeId == null)
        {
            throw new NotFoundException(
                "Employee profile not found");
        }

        if (payroll.EmployeeId != employeeId)
        {
            throw new BusinessException(
                "You can only access your own payslip");
        }

        return BuildPayslip(payroll);
    }

    private byte[] BuildPayslip(Models.Entities.Payroll payroll)
    {
        // Calculate Total Earnings for the summary
        var totalEarnings = payroll.BasicComponent
                            + payroll.HraComponent
                            + payroll.SpecialAllowanceComponent
                            + payroll.MedicalAllowanceComponent
                            + payroll.TravelAllowanceComponent
                            + payroll.Bonus;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));

                // 1. HEADER SECTION
                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("HRMS").FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                            c.Item().Text("Guntur,AndhraPradesh, 522002").FontSize(10).FontColor(Colors.Grey.Medium);
                        });
                        row.ConstantItem(150).AlignRight().Text("PAYSLIP").FontSize(24).Bold().FontColor(Colors.Grey.Lighten1);
                    });

                    col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                    col.Item().PaddingTop(10).AlignCenter()
                       .Text($"Payslip for the month of {payroll.PayMonth:D2} / {payroll.PayYear}")
                       .FontSize(12).Bold();
                });

                // 2. CONTENT SECTION
                page.Content().PaddingVertical(20).Column(column =>
                {
                    column.Spacing(20);

                    // --- Employee Details Grid ---
                    column.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        // Row 1
                        table.Cell().Text("Employee Name:").Bold();
                        table.Cell().Text($"{payroll.Employee?.FirstName} {payroll.Employee?.LastName}");
                        table.Cell().Text("Employee Code:").Bold();
                        table.Cell().Text($"{payroll.Employee?.EmployeeCode}");

                        // Row 2
                        table.Cell().PaddingTop(5).Text("Department:").Bold();
                        table.Cell().PaddingTop(5).Text($"{payroll.Employee?.Department?.Name}");
                        table.Cell().PaddingTop(5).Text("Designation:").Bold();
                        table.Cell().PaddingTop(5).Text($"{payroll.Employee?.Designation}");

                        // Row 3
                        table.Cell().PaddingTop(5).Text("Date of Joining:").Bold();
                        table.Cell().PaddingTop(5).Text("-"); // Replace with actual property if exists
                        table.Cell().PaddingTop(5).Text("UAN / PAN:").Bold();
                        table.Cell().PaddingTop(5).Text("-"); // Replace with actual property if exists
                    });

                    // --- Earnings & Deductions Table ---
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Earning Description
                            columns.RelativeColumn(2); // Earning Amount
                            columns.RelativeColumn(3); // Deduction Description
                            columns.RelativeColumn(2); // Deduction Amount
                        });

                        // Table Headers
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten3).Padding(5).Text("EARNINGS").Bold();
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("AMOUNT").Bold();
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten3).Padding(5).Text("DEDUCTIONS").Bold();
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("AMOUNT").Bold();

                        // Row: Basic & LOP
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("Basic");
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"{payroll.BasicComponent:N2}");
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("LOP Deduction");
                        table.Cell().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"{payroll.LopDeduction:N2}");

                        // Row: HRA & Empty
                        table.Cell().BorderLeft(1).BorderRight(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("House Rent Allowance (HRA)");
                        table.Cell().BorderRight(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"{payroll.HraComponent:N2}");
                        table.Cell().BorderRight(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("");
                        table.Cell().BorderRight(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text("");

                        // Row: Special Allowance
                        table.Cell().BorderLeft(1).BorderRight(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("Special Allowance");
                        table.Cell().BorderRight(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"{payroll.SpecialAllowanceComponent:N2}");
                        table.Cell().BorderRight(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("");
                        table.Cell().BorderRight(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text("");

                        // Row: Medical Allowance
                        table.Cell().BorderLeft(1).BorderRight(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("Medical Allowance");
                        table.Cell().BorderRight(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"{payroll.MedicalAllowanceComponent:N2}");
                        table.Cell().BorderRight(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("");
                        table.Cell().BorderRight(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text("");

                        // Row: Travel Allowance
                        table.Cell().BorderLeft(1).BorderRight(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("Travel Allowance");
                        table.Cell().BorderRight(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"{payroll.TravelAllowanceComponent:N2}");
                        table.Cell().BorderRight(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("");
                        table.Cell().BorderRight(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text("");

                        // Row: Bonus
                        table.Cell().Border(1).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("Bonus");
                        table.Cell().Border(1).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"{payroll.Bonus:N2}");
                        table.Cell().Border(1).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text("");
                        table.Cell().Border(1).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text("");

                        // Totals Row
                        table.Cell().Border(1).BorderColor(Colors.Grey.Darken1).Background(Colors.Grey.Lighten4).Padding(5).Text("Gross Earnings").Bold();
                        table.Cell().Border(1).BorderColor(Colors.Grey.Darken1).Background(Colors.Grey.Lighten4).Padding(5).AlignRight().Text($"{totalEarnings:N2}").Bold();
                        table.Cell().Border(1).BorderColor(Colors.Grey.Darken1).Background(Colors.Grey.Lighten4).Padding(5).Text("Total Deductions").Bold();
                        table.Cell().Border(1).BorderColor(Colors.Grey.Darken1).Background(Colors.Grey.Lighten4).Padding(5).AlignRight().Text($"{payroll.Deductions:N2}").Bold();
                    });

                    // --- Net Pay Block ---
                    column.Item().PaddingTop(10).Background(Colors.Blue.Darken2).Padding(15).Row(row =>
                    {
                        row.RelativeItem().AlignLeft().AlignMiddle().Text("NET PAYABLE AMOUNT").FontColor(Colors.White).FontSize(14).Bold();
                        row.RelativeItem().AlignRight().AlignMiddle().Text($"{payroll.NetSalary:N2}").FontColor(Colors.White).FontSize(16).Bold();
                    });

                    // --- Status Info ---
                    column.Item().PaddingTop(10).Text($"Status: {payroll.Status}").FontSize(10).Italic().FontColor(Colors.Grey.Darken1);
                });

                // 3. FOOTER SECTION
                page.Footer().Column(col =>
                {
                    col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    col.Item().PaddingTop(5).AlignCenter()
                       .Text($"This is a computer-generated document. Generated At: {payroll.GeneratedAt:yyyy-MM-dd HH:mm}")
                       .FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });
        }).GeneratePdf();
    }
}
using HRMS.API.Models.Common;
using HRMS.API.Models.DTOs.Payroll;

namespace HRMS.API.Interfaces;

public interface IPayrollService
{
    Task<PayrollDetailDto> GeneratePayrollAsync(GeneratePayrollDto dto, CancellationToken cancellationToken = default);

    Task<BulkPayrollResponseDto> GenerateMonthlyPayrollAsync(GenerateMonthlyPayrollDto dto, CancellationToken cancellationToken = default);

    Task<PagedResponse<PayrollResponseDto>> GetPayrollsAsync(PayrollFilterDto filter, CancellationToken cancellationToken = default);

    Task<PagedResponse<PayrollResponseDto>> GetEmployeePayrollsAsync(Guid employeeId, PayrollFilterDto filter, CancellationToken cancellationToken = default);

    Task<PagedResponse<PayrollResponseDto>> GetMyPayrollsAsync(PayrollFilterDto filter, CancellationToken cancellationToken = default);

    Task<PayrollDetailDto> GetPayrollAsync(Guid payrollId, CancellationToken cancellationToken = default);

    Task ApprovePayrollAsync(Guid payrollId, CancellationToken cancellationToken = default);

    Task MarkPayrollPaidAsync(Guid payrollId, CancellationToken cancellationToken = default);

    Task<byte[]> ExportPayrollsAsync(PayrollFilterDto filter, CancellationToken cancellationToken = default);
}
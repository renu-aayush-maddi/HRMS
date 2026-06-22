namespace HRMS.API.Interfaces;

public interface IPayslipService
{
    Task<byte[]> GeneratePayslipAsync(
        Guid payrollId,
        CancellationToken cancellationToken = default);

    Task<byte[]> GenerateMyPayslipAsync(
        Guid payrollId,
        CancellationToken cancellationToken = default);
}
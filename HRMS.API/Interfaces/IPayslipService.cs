namespace HRMS.API.Interfaces;

public interface IPayslipService
{
    byte[] GeneratePayslip(
        Guid payrollId);

    byte[] GenerateMyPayslip(
        Guid payrollId,
        Guid userId);
}
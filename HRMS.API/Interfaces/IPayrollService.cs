using HRMS.API.Models.DTOs.Payroll;

namespace HRMS.API.Interfaces;

public interface IPayrollService
{
    void GeneratePayroll(GeneratePayrollDto dto);

    List<PayrollResponseDto> GetAllPayrolls();

    List<PayrollResponseDto> GetEmployeePayrolls(Guid employeeId);

    void ApprovePayroll(Guid payrollId);

    void MarkPayrollPaid(Guid payrollId);

    List<PayrollResponseDto> GetMyPayrolls(Guid userId);
}
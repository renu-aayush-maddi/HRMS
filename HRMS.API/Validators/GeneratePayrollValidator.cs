using FluentValidation;
using HRMS.API.Models.DTOs.Payroll;

namespace HRMS.API.Validators.Payroll;

public class GeneratePayrollValidator
    : AbstractValidator<GeneratePayrollDto>
{
    public GeneratePayrollValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty();

        RuleFor(x => x.PayMonth)
            .InclusiveBetween(1, 12);

        RuleFor(x => x.PayYear)
            .GreaterThanOrEqualTo(2020);
    }
}
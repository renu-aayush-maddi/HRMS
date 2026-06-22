using FluentValidation;
using HRMS.API.Models.DTOs.Deduction;

namespace HRMS.API.Validators;

public class CreateDeductionValidator : AbstractValidator<CreateDeductionDto>
{
    public CreateDeductionValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty();

        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.DeductionMonth)
            .InclusiveBetween(1, 12);

        RuleFor(x => x.DeductionYear)
            .InclusiveBetween(
                2020,
                DateTime.UtcNow.Year + 1);
    }
}
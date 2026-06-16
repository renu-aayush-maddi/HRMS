using FluentValidation;
using HRMS.API.Models.DTOs.EmployeeExperience;

namespace HRMS.API.Validators;


public class AddEmployeeExperienceValidator : AbstractValidator<AddEmployeeExperienceDto>
{
    public AddEmployeeExperienceValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Designation)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.StartDate)
            .NotNull();

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
    }
}
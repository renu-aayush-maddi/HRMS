using FluentValidation;
using HRMS.API.Models.DTOs.EmployeeEducation;

namespace HRMS.API.Validators;

public class AddEmployeeEducationValidator : AbstractValidator<AddEmployeeEducationDto>
{
    public AddEmployeeEducationValidator()
    {
        RuleFor(x => x.Degree).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Specialization).NotEmpty().MaximumLength(100);
        RuleFor(x => x.InstitutionName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.GraduationYear).InclusiveBetween(1950, DateTime.Now.Year);
        RuleFor(x => x.Percentage).InclusiveBetween(0, 100).When(x => x.Percentage.HasValue);
    }
}
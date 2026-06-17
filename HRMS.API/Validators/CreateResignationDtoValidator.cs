using FluentValidation;
using HRMS.API.Models.DTOs.Resignation;

namespace HRMS.API.Validators.EmployeeResignations;

public class CreateResignationDtoValidator: AbstractValidator<CreateResignationDto>
{
    public CreateResignationDtoValidator()
    {
        RuleFor(x => x.LastWorkingDate)
            .NotEmpty()
            .WithMessage("Last working date is required.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required.")
            .MaximumLength(1000)
            .WithMessage("Reason cannot exceed 1000 characters.");
    }
}
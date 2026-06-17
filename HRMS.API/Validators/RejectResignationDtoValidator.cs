using FluentValidation;
using HRMS.API.Models.DTOs.Resignation;

namespace HRMS.API.Validators.EmployeeResignations;

public class RejectResignationDtoValidator: AbstractValidator<RejectResignationDto>
{
    public RejectResignationDtoValidator()
    {
        RuleFor(x => x.HrComments)
            .NotEmpty()
            .WithMessage("HR comments are required.")
            .MaximumLength(1000)
            .WithMessage("HR comments cannot exceed 1000 characters.");
    }
}
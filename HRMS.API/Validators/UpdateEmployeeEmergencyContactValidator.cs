using FluentValidation;
using HRMS.API.Models.DTOs.EmployeeEmergencyContact;

namespace HRMS.API.Validators;

public class UpdateEmployeeEmergencyContactValidator : AbstractValidator<UpdateEmployeeEmergencyContactDto>
{
    public UpdateEmployeeEmergencyContactValidator()
    {
        RuleFor(x => x.ContactName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Relationship).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Phone)
            .NotEmpty()
            .Matches(@"^\+?[0-9]{10,15}$")
            .WithMessage("Invalid phone number format.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}


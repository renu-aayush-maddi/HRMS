using FluentValidation;
using HRMS.API.Models.DTOs.EmployeeEmergencyContact;

namespace HRMS.API.Validators;

public class AddEmployeeEmergencyContactValidator : AbstractValidator<AddEmployeeEmergencyContactDto>
{
    public AddEmployeeEmergencyContactValidator()
    {
        RuleFor(x => x.ContactName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Relationship).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Phone)
            .NotEmpty()
            .Matches(@"^\+?[0-9]{10,15}$")
            .WithMessage("Invalid phone number format.Should be Between 10 to 15");
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}


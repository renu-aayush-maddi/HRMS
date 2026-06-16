using FluentValidation;
using HRMS.API.Common.Constants;
using HRMS.API.Models.DTOs.EmployeeAddress;

namespace HRMS.API.Validators;

public class AddEmployeeAddressValidator : AbstractValidator<AddEmployeeAddressDto>
{
    public AddEmployeeAddressValidator()
    {
        RuleFor(x => x.AddressLine1).NotEmpty().MaximumLength(300);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.State).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PostalCode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.AddressType)
            .NotEmpty()
            .Must(type => AddressTypes.All.Contains(type))
            .WithMessage($"AddressType must be one of: {string.Join(", ", AddressTypes.All)}");
    }
}
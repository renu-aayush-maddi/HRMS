using FluentValidation;
using HRMS.API.Models.DTOs.Resignation;

namespace HRMS.API.Validators.EmployeeResignations;

public class ResignationFilterDtoValidator
    : AbstractValidator<ResignationFilterDto>
{
    public ResignationFilterDtoValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x)
            .Must(x =>
                !x.FromResignationDate.HasValue ||
                !x.ToResignationDate.HasValue ||
                x.FromResignationDate <= x.ToResignationDate)
            .WithMessage(
                "From resignation date must be less than or equal to To resignation date.");
    }
}
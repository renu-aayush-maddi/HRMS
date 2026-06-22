using FluentValidation;
using HRMS.API.Models.DTOs.Review;

namespace HRMS.API.Validators;

public class AddReviewValidator : AbstractValidator<AddReviewDto>
{
    public AddReviewValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty();

        RuleFor(x => x.PerformanceCycleId)
            .NotEmpty();

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5);

        RuleFor(x => x.Comments)
            .MaximumLength(2000);
    }
}
using FluentValidation;
using HRMS.API.Models.DTOs.EmployeeDocument;
using HRMS.API.Common.Constants;
using HRMS.API.Constants;

namespace HRMS.API.Validators;

public class AddEmployeeDocumentValidator : AbstractValidator<AddEmployeeDocumentDto>
{
    private const long MaxFileSize = 10 * 1024 * 1024;
    private readonly string[] allowedExtensions = [".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx"];

    public AddEmployeeDocumentValidator()
    {
        RuleFor(x => x.DocumentType)
            .NotEmpty()
            .MaximumLength(100)
            .Must(type => DocumentTypes.All.Contains(type))
            .WithMessage($"DocumentType must be one of: {string.Join(", ", DocumentTypes.All)}");

        RuleFor(x => x.File)
     .NotNull()
     .WithMessage("File is required.")
     .DependentRules(() =>
     {
         RuleFor(x => x.File.Length)
             .LessThanOrEqualTo(MaxFileSize)
             .WithMessage("File size cannot exceed 10 MB.");

         RuleFor(x => x.File.FileName)
             .Must(HaveAllowedExtension)
             .WithMessage("Invalid file type.");
     });
    }

    private bool HaveAllowedExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return allowedExtensions.Contains(extension);
    }
}
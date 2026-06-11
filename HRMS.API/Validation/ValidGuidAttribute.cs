using System.ComponentModel.DataAnnotations;

namespace HRMS.API.Validations;

public class ValidGuidAttribute: ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if(value is Guid guid)
        {
            return guid != Guid.Empty;
        }

        return false;
    }
}
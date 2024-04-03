using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Utility;

public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly int _maxSize;
    public MaxFileSizeAttribute(int maxSize)
    {
        _maxSize = maxSize;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        IFormFile? file = value as IFormFile;

        if (file is not null)
        {
            if (file.Length > 1024 * 1024)
            {
                return new ValidationResult($"The maximum file size is {_maxSize} MB!");
            }
        }

        return ValidationResult.Success;
    }
}

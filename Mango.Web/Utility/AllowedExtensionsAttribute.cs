using System.ComponentModel.DataAnnotations;

namespace Mango.Web.Utility;

public class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _extensions;
    public AllowedExtensionsAttribute(string[] extensions)
    {
        _extensions = extensions;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        IFormFile? file = value as IFormFile;

        if (file is not null)
        {
            var extension = Path.GetExtension(file.FileName);
            if (!_extensions.Any(e => e.ToLower() == extension.ToLower()))
            {
                return new ValidationResult("The file extension is not allowed!");
            }
        }

        return ValidationResult.Success;
    }
}

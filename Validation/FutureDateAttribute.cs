using System.ComponentModel.DataAnnotations;

namespace TestCase.Validation
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date.Date < DateTime.UtcNow.Date)
                {
                    return new ValidationResult("Date must be in the future");
                }
            }
            return ValidationResult.Success;
        }
    }
}

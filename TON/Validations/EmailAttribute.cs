using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TON.Validations
{
    public class EmailAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult("Email is required");

            var email = value.ToString();
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (!Regex.IsMatch(email!, emailRegex))
                return new ValidationResult("Invalid email format");

            return ValidationResult.Success;
        }
    }
    }
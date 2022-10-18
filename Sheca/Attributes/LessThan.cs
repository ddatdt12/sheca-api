using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;

namespace Sheca.Attributes
{
    public class LessThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;
        private readonly bool _acceptNull;

        public LessThanAttribute(string comparisonProperty, bool acceptNull = false)
        {
            _comparisonProperty = comparisonProperty;
            _acceptNull = acceptNull;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (value == null)
            {
                return ValidationResult.Success;
            }
            if (property == null)
                throw new ArgumentException("Property with this name not found");

            //if (property.GetType() != value?.GetType())
            //    throw new ArgumentException("Type not match");

            dynamic? comparisonValue = property.GetValue(validationContext.ObjectInstance);
            if (_acceptNull && comparisonValue == null) { return ValidationResult.Success; }

            if ((DateTime)value < comparisonValue)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage);
        }
    }
}

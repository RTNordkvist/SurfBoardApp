using System;
using System.ComponentModel.DataAnnotations;

namespace SurfBoardApp
{
    //Ståljet fra Stackoverflow
    // This class represents a custom validation attribute that checks if the value of a property is greater than another property's value
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _startDatePropertyName;
        // Constructor that initializes the attribute with the name of the property to compare against
        public DateGreaterThanAttribute(string startDatePropertyName)
        {
            _startDatePropertyName = startDatePropertyName;
        }

        // Overrides the IsValid method of the base ValidationAttribute class to perform the validation
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Get the PropertyInfo object for the property to compare against
            var property = validationContext.ObjectType.GetProperty(_startDatePropertyName);
            if (property == null)
            {
                return new ValidationResult($"Property {_startDatePropertyName} not found");
            }

            // Get the value of the property to compare against
            var startDateValue = property.GetValue(validationContext.ObjectInstance, null);
            if (startDateValue == null)
            {
                return new ValidationResult($"Property {_startDatePropertyName} has no value");
            }

            // Check if the value to validate is null, in which case the validation is skipped
            if (value == null)
            {
                return ValidationResult.Success;
            }

            // Compare the values of the two properties and return a validation result based on the comparison
            if ((DateTime)value <= (DateTime)startDateValue)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }

}

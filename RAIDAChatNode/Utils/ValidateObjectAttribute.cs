using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RAIDAChatNode.Utils
{
    public class ValidateObjectAttribute: ValidationAttribute {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            var context = new ValidationContext(value, null, null);

            try
            {
                Validator.ValidateObject(value, context, true);
            }
            catch (Exception ex)
            {
                return new ValidationResult(ex.Message);
            }
            return ValidationResult.Success;
        }
    }
}
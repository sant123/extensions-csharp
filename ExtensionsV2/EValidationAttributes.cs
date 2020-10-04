namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Especifica que el valor coincida con la enumeración.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidateEnumAttribute : ValidationAttribute
    {
        /// <summary>
        /// Permite saber si el valor proporcionado es valido.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var enumType = value.GetType();

            return !Enum.IsDefined(enumType, value) ?
                new ValidationResult($"{value} is not a valid value for type {enumType.Name}") :
                ValidationResult.Success;
        }
    }
}

namespace Dataport.Terminfinder.BusinessObject.Validators;

/// <summary>
/// Check, if the Date are today or in the future
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DateTodayOrFutureAttribute : ValidationAttribute
{
    /// <summary>
    /// IsValid
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null || ((DateTime)value) == DateTime.MinValue)
        {
            return null;
        }

        return ((DateTime)value).Date < (DateTime.Now).Date 
            ? new ValidationResult(FormatErrorMessage(validationContext.DisplayName)) 
            : null;
    }
}
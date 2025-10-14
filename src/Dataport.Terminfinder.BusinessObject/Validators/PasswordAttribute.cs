namespace Dataport.Terminfinder.BusinessObject.Validators;

/// <summary>
/// Check, if the password is valid
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class PasswordAttribute : ValidationAttribute
{
    /// <summary>
    /// Check if the password is valid.
    /// </summary>
    /// <param name="value">the value to validate</param>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is not string password)
        {
            return null;
        }

        return IsPasswordValid(password) 
            ? null 
            : new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
    }

    private static bool IsPasswordValid(string password)
    {
        return PasswordValidator.IsValid(password);
    }
}
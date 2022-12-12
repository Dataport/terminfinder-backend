namespace Dataport.Terminfinder.Common.Extension;

/// <summary>
/// Enum extension methods
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Converts string to enum value (opposite to Enum.ToString()).
    /// </summary>
    /// <typeparam name="T">Type of the enum to convert the string into.</typeparam>
    /// <param name="s">string to convert to enum value.</param>
    public static T ToEnum<T>(this string s) where T : struct
    {
        return ToEnum<T>(s, ignoreCase: true);
    }

    /// <summary>
    /// Converts string to enum value (opposite to Enum.ToString()).
    /// </summary>
    /// <typeparam name="T">Type of the enum to convert the string into.</typeparam>
    /// <param name="s">string to convert to enum value.</param>
    /// <param name="ignoreCase">ignore case</param>
    public static T ToEnum<T>(this string s, bool ignoreCase) where T : struct
    {
        return Enum.TryParse(value: s, ignoreCase: ignoreCase, result: out T newValue) ? newValue : default;
    }
}
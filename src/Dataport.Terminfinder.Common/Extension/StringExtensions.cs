namespace Dataport.Terminfinder.Common.Extension;

/// <summary>
/// StringExtensions
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Convert the first character of a string to lower case
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [ContractAnnotation("value: notnull => notnull; value: null => null")]
    public static string FirstCharacterToLower(this string value)
    {
        if (string.IsNullOrEmpty(value) || Char.IsLower(value, 0))
        {
            return value;
        }

        return Char.ToLowerInvariant(value[0]) + value[1..];
    }
}
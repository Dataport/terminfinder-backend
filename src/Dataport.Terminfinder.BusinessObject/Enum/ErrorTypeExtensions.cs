namespace Dataport.Terminfinder.BusinessObject.Enum;

/// <summary>
/// Error type extensions methods
/// </summary>
public static class ErrorTypeExtensions
{
    private static readonly string LocalisationIdPrefix = "ErrorCode";

    /// <summary>
    /// Convert the value to the identifier in the localized resource files
    /// </summary>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public static string ToLocalisationId(this ErrorType enumValue)
    {
        return $"{LocalisationIdPrefix}{string.Format("{0:d4}", (int)enumValue)}";
    }

    /// <summary>
    /// Format the value to an error number
    /// </summary>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public static string ToFormattedErrorNumber(this ErrorType enumValue)
    {
        return $"{string.Format("{0:d4}", (int)enumValue)}";
    }
}
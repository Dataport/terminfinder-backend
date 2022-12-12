namespace Dataport.Terminfinder.BusinessObject.Error;

/// <summary>
/// Error return values
/// </summary>
public interface IErrorResult
{
    /// <summary>
    /// Error number
    /// </summary>
    /// <example>0000</example>
    string Code { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    /// <example>This is an example error message.</example>
    string Message { get; set; }

    /// <summary>
    /// Language code of the error message
    /// </summary>
    /// <example>de-DE</example>
    string Language { get; set; }

}
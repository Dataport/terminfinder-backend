using Dataport.Terminfinder.BusinessObject.Enum;
using System.Diagnostics.CodeAnalysis;

namespace Dataport.Terminfinder.WebAPI.Exceptions;

/// <summary>
/// RestApiException exception
/// </summary>
[Serializable]
[ExcludeFromCodeCoverage]
public abstract class RestApiException : Exception
{
    /// <summary>
    /// Error code
    /// </summary>
    public ErrorType ErrorCode { get; }

    /// <inheritdoc />
    protected RestApiException(string message, ErrorType errorCode)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <inheritdoc />
    protected RestApiException(string message, ErrorType errorCode, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
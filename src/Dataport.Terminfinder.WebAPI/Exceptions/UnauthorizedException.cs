using Dataport.Terminfinder.BusinessObject.Enum;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable UnusedMember.Global
namespace Dataport.Terminfinder.WebAPI.Exceptions;

/// <summary>
/// Unauthorized exception
/// </summary>
[Serializable]
[ExcludeFromCodeCoverage]
public class UnauthorizedException : RestApiException
{
    /// <inheritdoc />
    public UnauthorizedException(ErrorType errorCode) : base(string.Empty, errorCode)
    {
    }

    /// <inheritdoc />
    public UnauthorizedException(string message, ErrorType errorCode) : base(message, errorCode)
    {
    }

    /// <inheritdoc />
    public UnauthorizedException(string message, ErrorType errorCode, Exception innerException) : base(message,
        errorCode, innerException)
    {
    }
}
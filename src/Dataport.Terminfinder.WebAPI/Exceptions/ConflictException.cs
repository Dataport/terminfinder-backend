using Dataport.Terminfinder.BusinessObject.Enum;

// ReSharper disable UnusedMember.Global
namespace Dataport.Terminfinder.WebAPI.Exceptions;

/// <summary>
/// Not acceptable exception
/// </summary>
[Serializable]
public class ConflictException : RestApiException
{
    /// <inheritdoc />
    public ConflictException(ErrorType errorCode) : base(string.Empty, errorCode)
    {
    }

    /// <inheritdoc />
    public ConflictException(string message, ErrorType errorCode) : base(message, errorCode)
    {
    }

    /// <inheritdoc />
    public ConflictException(string message, ErrorType errorCode, Exception innerException) : base(message, errorCode,
        innerException)
    {
    }
}
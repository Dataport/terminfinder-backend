using Dataport.Terminfinder.BusinessObject.Enum;
using System.Runtime.Serialization;

// ReSharper disable UnusedMember.Global
namespace Dataport.Terminfinder.WebAPI.Exceptions;

/// <summary>
/// Bad request exception
/// </summary>
[Serializable]
public class BadRequestException : RestApiException
{
    /// <summary>
    /// Localized error message
    /// </summary>
    public string LocalizedErrorMessage { get; }

    /// <inheritdoc />
    public BadRequestException(string localizedErrorMessage, ErrorType errorCode)
        : base(localizedErrorMessage, errorCode)
    {
        LocalizedErrorMessage = localizedErrorMessage;
    }

    /// <inheritdoc />
    public BadRequestException(string localizedErrorMessage, ErrorType errorCode, Exception innerException)
        : base(localizedErrorMessage, errorCode, innerException)
    {
        LocalizedErrorMessage = localizedErrorMessage;
    }

    /// <inheritdoc />
    protected BadRequestException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
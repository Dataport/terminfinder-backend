using Dataport.Terminfinder.BusinessObject.Enum;
using System.Runtime.Serialization;

// ReSharper disable UnusedMember.Global
namespace Dataport.Terminfinder.WebAPI.Exceptions;

/// <summary>
/// Not found exception
/// </summary>
[Serializable]
public class NotFoundException : RestApiException
{
    /// <inheritdoc />
    public NotFoundException(ErrorType errorCode) : base(string.Empty, errorCode)
    {
    }

    /// <inheritdoc />
    public NotFoundException(string message, ErrorType errorCode) : base(message, errorCode)
    {
    }

    /// <inheritdoc />
    public NotFoundException(string message, ErrorType errorCode, Exception innerException) : base(message, errorCode,
        innerException)
    {
    }

    /// <inheritdoc />
    protected NotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
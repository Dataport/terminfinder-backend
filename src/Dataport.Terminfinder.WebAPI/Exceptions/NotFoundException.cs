using Dataport.Terminfinder.BusinessObject.Enum;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

// ReSharper disable UnusedMember.Global
namespace Dataport.Terminfinder.WebAPI.Exceptions;

/// <summary>
/// Not found exception
/// </summary>
[Serializable]
[ExcludeFromCodeCoverage]
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
    [Obsolete("Needs to be implemented for ISerializable.")]
    protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
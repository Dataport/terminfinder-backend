using Dataport.Terminfinder.BusinessObject.Enum;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

// ReSharper disable UnusedMember.Global
namespace Dataport.Terminfinder.WebAPI.Exceptions;

/// <summary>
/// Not acceptable exception
/// </summary>
[Serializable]
[ExcludeFromCodeCoverage]
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

    /// <inheritdoc />
    [Obsolete("Needs to be implemented for ISerializable.")]
    protected ConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
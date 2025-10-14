using Dataport.Terminfinder.BusinessObject.Enum;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

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

    /// <inheritdoc />
    [Obsolete("Needs to be implemented for ISerializable.")]
    protected RestApiException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        ArgumentNullException.ThrowIfNull(info);

        ErrorCode = (ErrorType)info.GetValue(nameof(ErrorCode), typeof(ErrorType))!;
    }

    /// <inheritdoc />
    [Obsolete("Needs to be implemented for ISerializable.")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info);

        info.AddValue(nameof(ErrorCode), ErrorCode);
        base.GetObjectData(info, context);
    }
}
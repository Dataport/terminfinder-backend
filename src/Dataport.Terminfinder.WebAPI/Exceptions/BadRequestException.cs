using Dataport.Terminfinder.BusinessObject.Enum;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

// ReSharper disable UnusedMember.Global
namespace Dataport.Terminfinder.WebAPI.Exceptions;

/// <summary>
/// Bad request exception
/// </summary>
[Serializable]
[ExcludeFromCodeCoverage]
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
    [Obsolete("Needs to be implemented for ISerializable.")]
    protected BadRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        ArgumentNullException.ThrowIfNull(info);

        LocalizedErrorMessage = (string)info.GetValue(nameof(LocalizedErrorMessage), typeof(string))!;
    }

    /// <inheritdoc />
    [Obsolete("Needs to be implemented for ISerializable.")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        ArgumentNullException.ThrowIfNull(info);

        info.AddValue(nameof(LocalizedErrorMessage), LocalizedErrorMessage);
        base.GetObjectData(info, context);
    }
}
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Dataport.Terminfinder.WebAPI.Exceptions;

/// <summary>
/// Basic authentication payload value could not be decoded
/// </summary>
[Serializable]
[ExcludeFromCodeCoverage]
public class DecodingBasicAuthenticationValueFailedException : Exception
{
    /// <inheritdoc />
    public DecodingBasicAuthenticationValueFailedException(string message) : base(message)
    {
    }

    /// <inheritdoc />
    public DecodingBasicAuthenticationValueFailedException(string message, Exception innerException) : base(message,
        innerException)
    {
    }

    /// <inheritdoc />
    [Obsolete("Needs to be implemented for ISerializable.")]
    protected DecodingBasicAuthenticationValueFailedException(SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
    }
}
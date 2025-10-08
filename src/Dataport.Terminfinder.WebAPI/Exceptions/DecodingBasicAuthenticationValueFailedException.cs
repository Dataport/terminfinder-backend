using System.Diagnostics.CodeAnalysis;

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
}
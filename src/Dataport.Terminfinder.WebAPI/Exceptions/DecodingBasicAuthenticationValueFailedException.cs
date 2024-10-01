namespace Dataport.Terminfinder.WebAPI.Exceptions;

/// <summary>
/// Basic authentication payload value could not be decoded
/// </summary>
[Serializable]
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
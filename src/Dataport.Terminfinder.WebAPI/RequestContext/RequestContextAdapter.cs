using Dataport.Terminfinder.BusinessObject;

namespace Dataport.Terminfinder.WebAPI.RequestContext;

/// <inheritdoc />
public sealed class RequestContextAdapter : IRequestContext
{
    [NotNull] private readonly IHttpContextAccessor _accessor;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="accessor"></param>
    public RequestContextAdapter(IHttpContextAccessor accessor)
    {
        _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
    }

    /// <inheritdoc />
    public UserCredential GetDecodedBasicAuthCredentials()
    {
        string basicAuthHeaderValue = _accessor.HttpContext?.Request.Headers[HttpHeaderConstants.BasicAuthentication];

        if (basicAuthHeaderValue == null)
        {
            return null;
        }

        return BasicAuthenticationValueEncoder.Decode(basicAuthHeaderValue);
    }
}
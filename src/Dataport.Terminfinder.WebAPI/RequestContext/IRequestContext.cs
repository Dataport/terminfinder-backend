using Dataport.Terminfinder.BusinessObject;
using Dataport.Terminfinder.WebAPI.Exceptions;

namespace Dataport.Terminfinder.WebAPI.RequestContext;

/// <summary>
/// Adapter to get the context like the values of http header i.e. like the username and password
/// </summary>
public interface IRequestContext
{
    /// <summary>
    /// Decoded username and password from the basic authentication http header of the request. The result can be null, if there is no basic auth header.
    /// </summary>
    /// <exception cref="DecodingBasicAuthenticationValueFailedException">wil be thrown, if the basic authentication value could not be decoded</exception>
    /// <returns>the decoded username and password</returns>
    [CanBeNull]
    UserCredential GetDecodedBasicAuthCredentials();
}
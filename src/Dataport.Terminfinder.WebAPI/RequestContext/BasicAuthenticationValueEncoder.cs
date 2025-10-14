using Dataport.Terminfinder.BusinessObject;
using Dataport.Terminfinder.WebAPI.Exceptions;
using System.Text;

namespace Dataport.Terminfinder.WebAPI.RequestContext;

/// <summary>
/// Decoder to decode and encode basic authentication payload values
/// </summary>
public class BasicAuthenticationValueEncoder
{
    private static readonly string PrefixWithSpace = "Basic ";

    /// <summary>
    /// Encode the submitted username and password and create the basic authentication payload value (see also RFC 2617).
    /// The username must not include a colon (s. https://tools.ietf.org/html/rfc2617#section-2).
    /// </summary>
    /// <param name="username">the username to encode. The username must not include a colon.</param>
    /// <param name="password">the password to encode. The password can include a colon.</param>
    /// <returns>the basic authentication payload value</returns>
    public string Encode(string username, string password)
    {
        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(password);

        return string.Concat(PrefixWithSpace,
            Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password)));
    }

    /// <summary>
    /// Decode username and password from the submitted basic authentication payload value.
    /// The structure is "Basic BASE64VALUE". The "BASE64VALUE" will be ASCII decoded and has the following format: 'username:password'.
    /// See also https://tools.ietf.org/html/rfc2617.
    /// </summary>
    /// <param name="encodedBasicAuthPayloadValue">the basic authentication payload value to decode.
    /// The structure is "Basic BASE64VALUE". The "BASE64VALUE" will be ASCII decoded and has the following format: 'username:password'</param>
    /// <exception cref="DecodingBasicAuthenticationValueFailedException">will be thrown if the basic authentication header could not be decoded</exception>
    /// <returns>the decoded username and password from the submitted basic authentication value</returns>
    public UserCredential Decode(string encodedBasicAuthPayloadValue)
    {
        ArgumentNullException.ThrowIfNull(encodedBasicAuthPayloadValue);

        if (!encodedBasicAuthPayloadValue.StartsWith(PrefixWithSpace))
        {
            throw new DecodingBasicAuthenticationValueFailedException(
                $"Value '{encodedBasicAuthPayloadValue}' is an invalid basic authentication value");
        }

        var encodedBasicAuthUsernamePasswordValue =
            encodedBasicAuthPayloadValue[PrefixWithSpace.Length..].Trim();
        string decodedBasicAuthUsernamePasswordValue;
        try
        {
            decodedBasicAuthUsernamePasswordValue =
                Encoding.ASCII.GetString(Convert.FromBase64String(encodedBasicAuthUsernamePasswordValue));
        }
        catch (Exception ex)
        {
            throw new DecodingBasicAuthenticationValueFailedException(
                $"Value '{encodedBasicAuthUsernamePasswordValue}' could not be decoded", ex);
        }

        var indexOfColon = decodedBasicAuthUsernamePasswordValue.IndexOf(':');
        if (indexOfColon < 0)
        {
            throw new DecodingBasicAuthenticationValueFailedException(
                $"Value '{decodedBasicAuthUsernamePasswordValue}' has not the expected format 'username:password'");
        }

        var username = decodedBasicAuthUsernamePasswordValue[..indexOfColon];
        var password = indexOfColon != 0
            ? decodedBasicAuthUsernamePasswordValue[(indexOfColon + 1)..]
            : string.Empty;
        return new UserCredential
        {
            Username = username,
            Password = password
        };
    }
}
namespace Dataport.Terminfinder.WebAPI.Tests.RequestContext;

[TestClass]
public class BasicAuthenticationEncoderTests
{
    [TestMethod]
    public void Decode_validBasicAuthenticationPayloadValue_resultContainsExpectedUsernamePassword()
    {
        var result = BasicAuthenticationValueEncoder.Decode("Basic dXNlcm5hbWU6UDQkJHcwcmQ=");
        Assert.AreEqual("username", result.Username);
        Assert.AreEqual("P4$$w0rd", result.Password);
    }

    [TestMethod]
    public void Decode_validBasicAuthenticationPayloadValueOnlyColonAsValue_resultContainsEmptyUsernameAndPassword()
    {
        var result = BasicAuthenticationValueEncoder.Decode("Basic Og==");
        Assert.AreEqual(string.Empty, result.Username);
        Assert.AreEqual(string.Empty, result.Password);
    }

    [TestMethod]
    public void Decode_validBasicAuthenticationPayloadValuePasswordContainsColon_resultContainsExpectedPassword()
    {
        // Value 'dXNlcm5hbWU6OlA0JCR3MHJkOg==' => 'username::P4$$w0rd:'
        var result = BasicAuthenticationValueEncoder.Decode("Basic dXNlcm5hbWU6OlA0JCR3MHJkOg==");
        Assert.AreEqual("username", result.Username);
        Assert.AreEqual(":P4$$w0rd:", result.Password);
    }

    [TestMethod]
    public void Decode_validBasicAuthenticationPayloadValuePasswordIsEmpty_resultContainsExpectedPassword()
    {
        // Value 'dXNlcm5hbWU6' => 'username:'
        var result = BasicAuthenticationValueEncoder.Decode("Basic dXNlcm5hbWU6");
        Assert.AreEqual("username", result.Username);
        Assert.AreEqual(string.Empty, result.Password);
    }

    [TestMethod]
    public void Decode_invalidBasicAuthenticationPayloadValueNoBasicAtBeginning_throwException()
    {
        Assert.ThrowsException<DecodingBasicAuthenticationValueFailedException>(() =>
            BasicAuthenticationValueEncoder.Decode(" dXNlcm5hbWU6UDQkJHcwcmQ="));
    }

    [TestMethod]
    public void Decode_invalidBasicAuthenticationPayloadValueNoColonInEncodedBasicAuthValue_throwException()
    {
        Assert.ThrowsException<DecodingBasicAuthenticationValueFailedException>(() =>
            BasicAuthenticationValueEncoder.Decode("Basic dXNlcm5hbWVQNCQkdzByZA=="));
    }

    [TestMethod]
    public void Decode_invalidBasicAuthenticationPayloadValueInvalidEncodedBasicAuthValue_throwException()
    {
        Assert.ThrowsException<DecodingBasicAuthenticationValueFailedException>(() =>
            BasicAuthenticationValueEncoder.Decode("Basic aaaaaaa"));
    }

    [TestMethod]
    public void Decode_null_throwException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => BasicAuthenticationValueEncoder.Decode(null));
    }

    [TestMethod]
    public void Encode_usernamePassword_validBasicAuthPayloadValue()
    {
        var result = BasicAuthenticationValueEncoder.Encode("username", "P4$$w0rd");
        Assert.AreEqual("Basic dXNlcm5hbWU6UDQkJHcwcmQ=", result);
    }
}
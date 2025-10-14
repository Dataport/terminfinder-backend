namespace Dataport.Terminfinder.WebAPI.Tests.RequestContext;

[TestClass]
public class BasicAuthenticationEncoderTests
{
    [TestMethod]
    public void Decode_validBasicAuthenticationPayloadValue_resultContainsExpectedUsernamePassword()
    {
        var sut = new BasicAuthenticationValueEncoder();
        var result = sut.Decode("Basic dXNlcm5hbWU6UDQkJHcwcmQ=");
        Assert.AreEqual("username", result.Username);
        Assert.AreEqual("P4$$w0rd", result.Password);
    }

    [TestMethod]
    public void Decode_validBasicAuthenticationPayloadValueOnlyColonAsValue_resultContainsEmptyUsernameAndPassword()
    {
        var sut = new BasicAuthenticationValueEncoder();
        var result = sut.Decode("Basic Og==");
        Assert.AreEqual(string.Empty, result.Username);
        Assert.AreEqual(string.Empty, result.Password);
    }

    [TestMethod]
    public void Decode_validBasicAuthenticationPayloadValuePasswordContainsColon_resultContainsExpectedPassword()
    {
        var sut = new BasicAuthenticationValueEncoder();
        // Value 'dXNlcm5hbWU6OlA0JCR3MHJkOg==' => 'username::P4$$w0rd:'
        var result = sut.Decode("Basic dXNlcm5hbWU6OlA0JCR3MHJkOg==");
        Assert.AreEqual("username", result.Username);
        Assert.AreEqual(":P4$$w0rd:", result.Password);
    }

    [TestMethod]
    public void Decode_validBasicAuthenticationPayloadValuePasswordIsEmpty_resultContainsExpectedPassword()
    {
        var sut = new BasicAuthenticationValueEncoder();
        // Value 'dXNlcm5hbWU6' => 'username:'
        var result = sut.Decode("Basic dXNlcm5hbWU6");
        Assert.AreEqual("username", result.Username);
        Assert.AreEqual(string.Empty, result.Password);
    }

    [TestMethod]
    public void Decode_invalidBasicAuthenticationPayloadValueNoBasicAtBeginning_throwException()
    {
        var sut = new BasicAuthenticationValueEncoder();
        Assert.ThrowsException<DecodingBasicAuthenticationValueFailedException>(() => sut.Decode(" dXNlcm5hbWU6UDQkJHcwcmQ="));
    }

    [TestMethod]
    public void Decode_invalidBasicAuthenticationPayloadValueNoColonInEncodedBasicAuthValue_throwException()
    {
        var sut = new BasicAuthenticationValueEncoder();
        Assert.ThrowsException<DecodingBasicAuthenticationValueFailedException>(() => sut.Decode("Basic dXNlcm5hbWVQNCQkdzByZA=="));
    }

    [TestMethod]
    public void Decode_invalidBasicAuthenticationPayloadValueInvalidEncodedBasicAuthValue_throwException()
    {
        var sut = new BasicAuthenticationValueEncoder();
        Assert.ThrowsException<DecodingBasicAuthenticationValueFailedException>(() => sut.Decode("Basic aaaaaaa"));
    }

    [TestMethod]
    public void Decode_null_throwException()
    {
        var sut = new BasicAuthenticationValueEncoder();
        Assert.ThrowsException<ArgumentNullException>(() => sut.Decode(null));
    }

    [TestMethod]
    public void Encode_usernamePassword_validBasicAuthPayloadValue()
    {
        var sut = new BasicAuthenticationValueEncoder();
        var result = sut.Encode("username", "P4$$w0rd");
        Assert.AreEqual("Basic dXNlcm5hbWU6UDQkJHcwcmQ=", result);
    }
}

namespace Dataport.Terminfinder.WebAPI.Tests.RequestContext;

[TestClass]
public class BasicAuthenticationEncoderTests
{
    [TestMethod]
    public void Decode_validBasicAuthenticationPayloadValue_resultContainsExpectedUsernamePassword()
    {
        var sut = new BasicAuthenticationValueEncoder();
        UserCredential result = sut.Decode("Basic dXNlcm5hbWU6UDQkJHcwcmQ=");
        Assert.AreEqual("username", result.Username);
        Assert.AreEqual("P4$$w0rd", result.Password);
    }

    [TestMethod]
    public void Decode_validBasicAuthenticationPayloadValueOnlyColonAsValue_resultContainsEmptyUsernameAndPassword()
    {
        var sut = new BasicAuthenticationValueEncoder();
        UserCredential result = sut.Decode("Basic Og==");
        Assert.AreEqual(string.Empty, result.Username);
        Assert.AreEqual(string.Empty, result.Password);
    }

    [TestMethod]
    public void Decode_validBasicAuthenticationPayloadValuePasswordContainsColon_resultContainsExpectedPassword()
    {
        var sut = new BasicAuthenticationValueEncoder();
        // Value 'dXNlcm5hbWU6OlA0JCR3MHJkOg==' => 'username::P4$$w0rd:'
        UserCredential result = sut.Decode("Basic dXNlcm5hbWU6OlA0JCR3MHJkOg==");
        Assert.AreEqual("username", result.Username);
        Assert.AreEqual(":P4$$w0rd:", result.Password);
    }

    [TestMethod]
    public void Decode_validBasicAuthenticationPayloadValuePasswordIsEmpty_resultContainsExpectedPassword()
    {
        var sut = new BasicAuthenticationValueEncoder();
        // Value 'dXNlcm5hbWU6' => 'username:'
        UserCredential result = sut.Decode("Basic dXNlcm5hbWU6");
        Assert.AreEqual("username", result.Username);
        Assert.AreEqual(string.Empty, result.Password);
    }

    [TestMethod]
    public void Decode_invalidBasicAuthenticationPayloadValueNoBasicAtBeginning_throwException()
    {
        var sut = new BasicAuthenticationValueEncoder();
        try
        {
            sut.Decode(" dXNlcm5hbWU6UDQkJHcwcmQ=");
            Assert.Fail("An exception should be thrown");
        }
        catch (DecodingBasicAuthenticationValueFailedException)
        {
        }
    }

    [TestMethod]
    public void Decode_invalidBasicAuthenticationPayloadValueNoColonInEncodedBasicAuthValue_throwException()
    {
        var sut = new BasicAuthenticationValueEncoder();
        try
        {
            // Value 'dXNlcm5hbWVQNCQkdzByZA==' => 'usernameP4$$w0rd'
            sut.Decode("Basic dXNlcm5hbWVQNCQkdzByZA==");
            Assert.Fail("An exception should be thrown");
        }
        catch (DecodingBasicAuthenticationValueFailedException)
        {
        }
    }

    [TestMethod]
    public void Decode_invalidBasicAuthenticationPayloadValueInvalidEncodedBasicAuthValue_throwException()
    {
        var sut = new BasicAuthenticationValueEncoder();
        try
        {
            sut.Decode("Basic aaaaaaa");
            Assert.Fail("An exception should be thrown");
        }
        catch (DecodingBasicAuthenticationValueFailedException)
        {
        }
    }

    [TestMethod]
    public void Decode_null_throwException()
    {
        var sut = new BasicAuthenticationValueEncoder();
        try
        {
            sut.Decode(null);
            Assert.Fail("An exception should be thrown");
        }
        catch (ArgumentNullException)
        {
        }
    }

    [TestMethod]
    public void Encode_usernamePassword_validBasicAuthPayloadValue()
    {
        var sut = new BasicAuthenticationValueEncoder();
        string result = sut.Encode("username", "P4$$w0rd");
        Assert.AreEqual("Basic dXNlcm5hbWU6UDQkJHcwcmQ=", result);
    }
}

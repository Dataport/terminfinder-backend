using Microsoft.Extensions.Options;

namespace Dataport.Terminfinder.WebAPI.Tests.Services.Configuration;

public static class ConfigValidatorTestUtils
{
    public static void CheckValidValidationResult(ValidateOptionsResult validateOptionsResult)
    {
        Assert.IsTrue(validateOptionsResult.Succeeded);
        Assert.AreEqual(ValidateOptionsResult.Success, validateOptionsResult);
        Assert.IsTrue(validateOptionsResult.Succeeded);
        Assert.IsNull(validateOptionsResult.FailureMessage);
    }

    public static void CheckInvalidValidationResult(
        ValidateOptionsResult validateOptionsResult,
        string expectedFailureMessage)
    {
        Assert.IsTrue(validateOptionsResult.Failed);
        Assert.IsFalse(validateOptionsResult.Succeeded);
        Assert.AreEqual(expectedFailureMessage, validateOptionsResult.FailureMessage);
    }
}
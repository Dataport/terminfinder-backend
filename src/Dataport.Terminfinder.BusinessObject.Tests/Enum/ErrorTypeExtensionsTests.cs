using Dataport.Terminfinder.BusinessObject.Enum;

namespace Dataport.Terminfinder.BusinessObject.Tests.Enum;

[TestClass]
[ExcludeFromCodeCoverage]
public class ErrorTypeExtensionsTests
{
    [TestMethod]
    public void ToLocalisationId_AnyValue_formattedEnumIntValue()
    {
        Assert.AreEqual("ErrorCode0010", ErrorType.GeneralError.ToLocalisationId());
    }

    [TestMethod]
    public void ToFormattedErrorNumber_AnyValue_formattedValue()
    {
        Assert.AreEqual("0010", ErrorType.GeneralError.ToFormattedErrorNumber());
    }
}
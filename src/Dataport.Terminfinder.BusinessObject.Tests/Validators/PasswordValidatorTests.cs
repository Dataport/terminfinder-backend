using Dataport.Terminfinder.BusinessObject.Validators;

namespace Dataport.Terminfinder.BusinessObject.Tests.Validators;

/// <summary>
/// Testclass for PasswordValidator
/// </summary>
[TestClass]
[ExcludeFromCodeCoverage]
public class PasswordValidatorTests
{
    [TestMethod]
    public void IsValid_ValidPassword_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1!"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii33_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1!"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii34_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1\""));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii35_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1#"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii36_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1$"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii37_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1%"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii38_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1&"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii39_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1'"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii40_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1("));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii41_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1)"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii42_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1*"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii43_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1+"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii44_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1,"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii45_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1-"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii46_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1."));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii47_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1/"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii58_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1:"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii59_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1;"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii60_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1<"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii61_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1="));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii62_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1>"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii63_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1?"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii64_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1@"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii91_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1["));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii92_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1\\"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii93_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1]"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii94_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1^"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii95_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1_"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii96_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1`"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii123_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1{"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii124_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1|"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii125_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1}"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordAscii126_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1~"));
    }

    [TestMethod]
    public void IsValid_InvalidPasswordTooLessCharacter_false()
    {
        Assert.IsFalse(PasswordValidator.IsValid("Blafa1!"));
    }

    [TestMethod]
    public void IsValid_InvalidPasswordMaxLengthExceeded_false()
    {
        Assert.IsFalse(PasswordValidator.IsValid("Blafas1!99000000000011111111112"));
    }

    [TestMethod]
    public void IsValid_ValidPasswordExactlyMaxLenght_true()
    {
        Assert.IsTrue(PasswordValidator.IsValid("Blafas1!9900000000001111111111"));
    }

    [TestMethod]
    public void IsValid_InvalidPasswordNoUpperLetter_false()
    {
        Assert.IsFalse(PasswordValidator.IsValid("blafas1!"));
    }

    [TestMethod]
    public void IsValid_InvalidPasswordNoDigit_false()
    {
        Assert.IsFalse(PasswordValidator.IsValid("Blafase!"));
    }

    [TestMethod]
    public void IsValid_InvalidPasswordNoSpecialCharacter_false()
    {
        Assert.IsFalse(PasswordValidator.IsValid("Blafas1e"));
    }

    [TestMethod]
    public void IsValid_null_throwException()
    {
        try
        {
            PasswordValidator.IsValid(null);
            Assert.Fail("An exception should be thrown");
        }
        catch (ArgumentNullException)
        {
        }
    }
}
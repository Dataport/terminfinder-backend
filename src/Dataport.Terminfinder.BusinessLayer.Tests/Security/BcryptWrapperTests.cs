using Dataport.Terminfinder.BusinessLayer.Security;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dataport.Terminfinder.BusinessLayer.Tests.Security;

[TestClass]
public class BcryptWrapperTests
{
    private ILogger<BcryptWrapper> logger;
    private ISaltGenerator saltGenerator;
    private readonly string SaltForUnittests = "$2b$10$bKHadGFqngTajUrRAozjxe";

    [TestInitialize]
    public void Initialize()
    {
        // fake logger
        logger = Mock.Of<ILogger<BcryptWrapper>>();
        var mockSaltGenerator = new Mock<ISaltGenerator>();
        mockSaltGenerator.Setup(g => g.GenerateSalt()).Returns(SaltForUnittests);
        saltGenerator = mockSaltGenerator.Object;
    }

    [TestMethod]
    public void HashPassword_null_throwException()
    {
        var sut = CreateSut(saltGenerator);
        Assert.ThrowsException<ArgumentNullException>(() => sut.HashPassword(null));
    }

    [TestMethod]
    [TestCategory(TestCategoryConstants.LongRunningTest)]
    public void HashPassword_anyPassword_createdBcryptHash()
    {
        var sut = CreateSut(saltGenerator);
        var resultPassword = sut.HashPassword("Blafasel1!");
        Assert.AreEqual("$2b$10$bKHadGFqngTajUrRAozjxe0LmYFbt3W3nZmAaPNEEFb6i8ZtGRgra", resultPassword);
    }

    [TestMethod]
    public void HashPassword_passwordIsNullHashIsNotNull_throwException()
    {
        var sut = CreateSut(saltGenerator);
        Assert.ThrowsException<ArgumentNullException>(() => sut.Verify(null, string.Empty));
    }

    [TestMethod]
    public void HashPassword_passwordIsNotNullPasswordIsNull_throwException()
    {
        var sut = CreateSut(saltGenerator);
        Assert.ThrowsException<ArgumentNullException>(() => sut.Verify(string.Empty, null));
    }

    [TestMethod]
    [TestCategory(TestCategoryConstants.LongRunningTest)]
    public void Verify_verificationSuccessful_true()
    {
        var sut = CreateSut(saltGenerator);
        // password hash from password 'Blafasel1!' with salt '$2b$10$bKHadGFqngTajUrRAozjxe'
        var passwordHashFromPassword = "$2b$10$bKHadGFqngTajUrRAozjxe0LmYFbt3W3nZmAaPNEEFb6i8ZtGRgra";
        var verifyResult = sut.Verify("Blafasel1!", passwordHashFromPassword);
        Assert.IsTrue(verifyResult);
    }

    [TestMethod]
    [TestCategory(TestCategoryConstants.LongRunningTest)]
    public void Verify_verificationFailed_false()
    {
        var sut = CreateSut(saltGenerator);
        // password hash from password 'Blafasel2!' with salt '$2b$10$bKHadGFqngTajUrRAozjxe'
        var passwordHashFromDifferentPassword = "$2b$10$bKHadGFqngTajUrRAozjxeEpWNoglbU.Ybb2M6pF5h233jsl4V0vq";
        var verifyResult = sut.Verify("Blafasel1!", passwordHashFromDifferentPassword);
        Assert.IsFalse(verifyResult);
    }

    private BcryptWrapper CreateSut(ISaltGenerator generator)
    {
        return new BcryptWrapper(generator, logger);
    }
}
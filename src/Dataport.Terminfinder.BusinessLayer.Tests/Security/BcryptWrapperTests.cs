using Dataport.Terminfinder.BusinessLayer.Security;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace Dataport.Terminfinder.BusinessLayer.Tests.Security;

[TestClass]
[ExcludeFromCodeCoverage]
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
    [ExpectedException(typeof(ArgumentNullException))]
    public void HashPassword_null_throwException()
    {
        BcryptWrapper sut = CreateSut(saltGenerator);
        sut.HashPassword(null);
    }

    [TestMethod]
    [TestCategory(TestCategoryConstants.LongRunningTest)]
    public void HashPassword_anyPassword_createdBcryptHash()
    {
        BcryptWrapper sut = CreateSut(saltGenerator);
        string resultPassword = sut.HashPassword("Blafasel1!");
        Assert.AreEqual("$2b$10$bKHadGFqngTajUrRAozjxe0LmYFbt3W3nZmAaPNEEFb6i8ZtGRgra", resultPassword);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void HashPassword_passwordIsNullHashIsNotNull_throwException()
    {
        BcryptWrapper sut = CreateSut(saltGenerator);
        sut.Verify(null, string.Empty);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void HashPassword_passwordIsNotNullPasswordIsNull_throwException()
    {
        BcryptWrapper sut = CreateSut(saltGenerator);
        sut.Verify(string.Empty, null);
    }

    [TestMethod]
    [TestCategory(TestCategoryConstants.LongRunningTest)]
    public void Verify_verificationSuccessful_true()
    {
        BcryptWrapper sut = CreateSut(saltGenerator);
        // password hash from password 'Blafasel1!' with salt '$2b$10$bKHadGFqngTajUrRAozjxe'
        string passwordHashFromPassword = "$2b$10$bKHadGFqngTajUrRAozjxe0LmYFbt3W3nZmAaPNEEFb6i8ZtGRgra";
        bool verifyResult = sut.Verify("Blafasel1!", passwordHashFromPassword);
        Assert.IsTrue(verifyResult);
    }

    [TestMethod]
    [TestCategory(TestCategoryConstants.LongRunningTest)]
    public void Verify_verificationFailed_false()
    {
        BcryptWrapper sut = CreateSut(saltGenerator);
        // password hash from password 'Blafasel2!' with salt '$2b$10$bKHadGFqngTajUrRAozjxe'
        string passwordHashFromDifferentPassword = "$2b$10$bKHadGFqngTajUrRAozjxeEpWNoglbU.Ybb2M6pF5h233jsl4V0vq";
        bool verifyResult = sut.Verify("Blafasel1!", passwordHashFromDifferentPassword);
        Assert.IsFalse(verifyResult);
    }

    private BcryptWrapper CreateSut(ISaltGenerator generator)
    {
        return new BcryptWrapper(generator, logger);
    }
}
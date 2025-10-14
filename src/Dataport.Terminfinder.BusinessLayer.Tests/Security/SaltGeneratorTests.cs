using Dataport.Terminfinder.BusinessLayer.Security;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dataport.Terminfinder.BusinessLayer.Tests.Security;

[TestClass]
public class SaltGeneratorTests
{
    private ILogger<SaltGenerator> logger;

    [TestInitialize]
    public void Initialize()
    {
        // fake logger
        logger = Mock.Of<ILogger<SaltGenerator>>();
    }

    [TestMethod]
    [TestCategory(TestCategoryConstants.LongRunningTest)]
    public void GenerateSalt_nothing_generatedSaltStartsWithExpectedBcryptPrefix()
    {
        var sut = CreateSut();
        var salt = sut.GenerateSalt();
        Assert.IsTrue(salt.StartsWith("$2b$10$"));
    }

    private SaltGenerator CreateSut()
    {
        return new SaltGenerator(logger);
    }
}
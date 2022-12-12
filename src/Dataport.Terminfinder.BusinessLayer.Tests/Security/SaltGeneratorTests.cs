using Dataport.Terminfinder.BusinessLayer.Security;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace Dataport.Terminfinder.BusinessLayer.Tests.Security;

[TestClass]
[ExcludeFromCodeCoverage]
public class SaltGeneratorTests
{
    private ILogger<SaltGenerator> logger;

    [TestInitialize]
    public void Inilialize()
    {
        // fake logger
        logger = Mock.Of<ILogger<SaltGenerator>>();
    }

    [TestMethod]
    [TestCategory(TestCategoryConstants.LongRunningTest)]
    public void GenerateSalt_nothing_generatedSaltStartsWithExpectedBcryptPrefix()
    {
        SaltGenerator sut = CreateSut();
        string salt = sut.GenerateSalt();
        Assert.IsTrue(salt.StartsWith("$2b$10$"));
    }

    private SaltGenerator CreateSut()
    {
        return new SaltGenerator(logger);
    }
}
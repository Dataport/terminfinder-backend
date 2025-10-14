using Dataport.Terminfinder.Repository.Tests.Utils;

namespace Dataport.Terminfinder.Repository.Tests;

[TestClass]
public class AppConfigRepositoryTests
{
    private ILogger<AppConfigRepository> _logger;

    [TestInitialize]
    public void Initialize()
    {
        // fake logger
        var mockLog = new Mock<ILogger<AppConfigRepository>>();
        _logger = mockLog.Object;
        _logger = Mock.Of<ILogger<AppConfigRepository>>();
    }

    [TestMethod]
    public void GetAppInfo_Okay()
    {

        string version = "1.2.3";
        string builddate = "2018-01-02";

        // https://medium.com/@metse/entity-framework-core-unit-testing-3c412a0a997c

        var appConfigs = new List<AppConfig>
        {
            new()
            {
                Key = "version",
                Value = version
            },
            new()
            {
                Key = "builddate",
                Value = builddate
            }
        };

        var mockSet = DbSetMockFactory.CreateMockDbSet(appConfigs);

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.AppConfig).Returns(mockSet.Object);

        // act fetch
        AppConfigRepository repository = new (mockContext.Object, _logger);
        AppInfo appInfo = repository.GetAppInfo();

        // Assert
        Assert.AreEqual(appInfo.VersionNumber, version);
        Assert.AreEqual(appInfo.BuildDate, builddate);
    }


    [TestMethod]
    public void GetAppInfo_Nullable()
    {

        string version = "1.2.3";
        string builddate = "2018-01-02";

        var appConfigs = new List<AppConfig>
        {
            new()
            {
                Key = "unkown",
                Value = version
            },
            new()
            {
                Key = "unknown2",
                Value = builddate
            }
        };

        var mockSet = DbSetMockFactory.CreateMockDbSet(appConfigs);

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.AppConfig).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        AppConfigRepository repository = new (mockContext.Object, _logger);
        AppInfo appInfo = repository.GetAppInfo();

        // Assert
        Assert.AreEqual(null, appInfo);
    }
}
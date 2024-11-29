namespace Dataport.Terminfinder.Repository.Tests;

[TestClass]
[ExcludeFromCodeCoverage]
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

        IQueryable<AppConfig> appConfigs = new List<AppConfig>
        {
            new AppConfig()
            {
                Key = "version",
                Value = version
            },
            new AppConfig()
            {
                Key = "builddate",
                Value = builddate
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<AppConfig>>();
        SetMockSetup(mockSet, appConfigs);

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

        IQueryable<AppConfig> appConfigs = new List<AppConfig>
        {
            new AppConfig()
            {
                Key = "unkown",
                Value = version
            },
            new AppConfig()
            {
                Key = "unknown2",
                Value = builddate
            }
        }.AsQueryable();

        // To query our database we need to implement IQueryable  
        var mockSet = new Mock<DbSet<AppConfig>>();
        SetMockSetup(mockSet, appConfigs);

        var mockContext = new Mock<DataContext>();
        mockContext.Setup(c => c.AppConfig).Returns(mockSet.Object);
        mockContext.Setup(c => c.SetTracking(It.IsAny<bool>()));

        // act fetch
        AppConfigRepository repository = new (mockContext.Object, _logger);
        AppInfo appInfo = repository.GetAppInfo();

        // Assert
        Assert.AreEqual(null, appInfo);
    }

    private static void SetMockSetup(Mock<DbSet<AppConfig>> mockSet, IQueryable<AppConfig> appConfigs)
    {
        mockSet.As<IQueryable<AppConfig>>().Setup(m => m.Provider).Returns(appConfigs.Provider);
        mockSet.As<IQueryable<AppConfig>>().Setup(m => m.Expression).Returns(appConfigs.Expression);
        mockSet.As<IQueryable<AppConfig>>().Setup(m => m.ElementType).Returns(appConfigs.ElementType);
        mockSet.As<IQueryable<AppConfig>>().Setup(m => m.GetEnumerator()).Returns(appConfigs.GetEnumerator());
    }
}
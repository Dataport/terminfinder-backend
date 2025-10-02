namespace Dataport.Terminfinder.WebAPI.Tests.Controllers;

[TestClass]
public class AppControllerTests
{
    private ILogger<AppController> _logger;
    private IStringLocalizer<AppController> _localizer;
    private IRequestContext _requestContext;

    [TestInitialize]
    public void Initialize()
    {
        // fake logger
        var mockLog = new Mock<ILogger<AppController>>();
        _logger = mockLog.Object;
        _logger = Mock.Of<ILogger<AppController>>();

        // fake localizer
        var mockLocalize = new Mock<IStringLocalizer<AppController>>();
        _localizer = mockLocalize.Object;
        _localizer = Mock.Of<IStringLocalizer<AppController>>();

        // fake request context
        _requestContext = Mock.Of<IRequestContext>();
    }

    [TestMethod]
    public void GetAppInfo_Okay()
    {
        string versionDate = "2010-10-30";
        string versionNumber = "1.2.3";

        AppInfo fakeAppInfo = new()
        {
            BuildDate = versionDate,
            VersionNumber = versionNumber
        };

        var mock = new Mock<IAppConfigBusinessLayer>();
        mock.Setup(m => m.GetAppInfo())
            .Returns(fakeAppInfo);

        var controller = new AppController(mock.Object, _requestContext, _logger, _localizer);

        // Act
        IActionResult httpResult = controller.Get();
        OkObjectResult result = httpResult as OkObjectResult;
        AppInfo app = result?.Value as AppInfo;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(app);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(app.VersionNumber, versionNumber);
        Assert.AreEqual(app.BuildDate, versionDate);
    }
}

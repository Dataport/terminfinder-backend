namespace Dataport.Terminfinder.WebAPI.Tests.Controllers;

[TestClass]
public class AppControllerTests
{
    [TestMethod]
    public void GetAppInfo_Okay()
    {
        var versionDate = "2010-10-30";
        var versionNumber = "1.2.3";

        AppInfo appInfo = new()
        {
            BuildDate = versionDate,
            VersionNumber = versionNumber
        };

        var mockAppConfigBusinessLayer = new Mock<IAppConfigBusinessLayer>();
        mockAppConfigBusinessLayer.Setup(m => m.GetAppInfo()).Returns(appInfo);

        var sut = CreateSut(mockAppConfigBusinessLayer.Object);

        // Act
        var httpResult = sut.Get();
        var result = httpResult as OkObjectResult;
        var app = result?.Value as AppInfo;

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(app);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(app.VersionNumber, versionNumber);
        Assert.AreEqual(app.BuildDate, versionDate);
    }

    [TestMethod]
    public void GetAppInfo_AppInfoIsNull_StatusCode500()
    {
        var mockAppConfigBusinessLayer = new Mock<IAppConfigBusinessLayer>();
        mockAppConfigBusinessLayer.Setup(m => m.GetAppInfo()).Returns((AppInfo)null);

        var sut = CreateSut(mockAppConfigBusinessLayer.Object);

        // Act
        var httpResult = sut.Get();
        var result = httpResult as ObjectResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(500, result.StatusCode);
    }

    private static AppController CreateSut(IAppConfigBusinessLayer appConfigBusinessLayer = null)
    {
        var appConfigBusinessLayerToUse = appConfigBusinessLayer ?? new Mock<IAppConfigBusinessLayer>().Object;
        var mockRequestContext = new Mock<IRequestContext>();
        var mockLogger = new Mock<ILogger<AppController>>();
        var mockLocalizer = new Mock<IStringLocalizer<AppController>>();

        return new AppController(
            appConfigBusinessLayerToUse,
            mockRequestContext.Object,
            mockLogger.Object,
            mockLocalizer.Object);
    }
}
namespace Dataport.Terminfinder.WebAPI.Tests.IntegrationTests;

[TestClass]
[TestCategory("Integrationtest")]
public class AppControllerIntegrationTests : BaseIntegrationTests
{
    private TestServer _testServer;
    private IHost _host;

    [TestInitialize]
    public async Task Initialize()
    {
        var config = GetConfigurationBuilder();
        _host = new HostBuilder()
            .ConfigureWebHost(builder =>
                {
                    builder
                        .UseTestServer()
                        .UseStartup<Startup>()
                        .UseConfiguration(config);
                }
            )
            .Build();
        await _host.StartAsync();
        _testServer = _host.GetTestServer();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        if (_testServer != null)
        {
            await _host.StopAsync();
            _testServer.Dispose();
        }
        _host?.Dispose();
    }

    [TestMethod]
    public async Task GetAppInfo_Okay()
    {
        var expectedAppInfo = new AppInfo
        {
            BuildDate = "2025-07-04",
            VersionNumber = "1.2.2"
        };

        var client = _testServer.CreateClient();

        // Act
        var response = await client.GetAsync($"app");
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        var responseText = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<AppInfo>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(dto);
        Assert.IsInstanceOfType(dto, typeof(AppInfo));
        Assert.AreEqual(expectedAppInfo.BuildDate, dto.BuildDate);
        Assert.AreEqual(expectedAppInfo.VersionNumber, dto.VersionNumber);
    }
}
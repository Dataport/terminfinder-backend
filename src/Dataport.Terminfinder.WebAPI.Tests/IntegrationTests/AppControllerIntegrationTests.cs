namespace Dataport.Terminfinder.WebAPI.Tests.IntegrationTests;

[TestClass]
[TestCategory("Integrationtest")]
[ExcludeFromCodeCoverage]
public class AppControllerIntegrationTests : BaseIntegrationTests
{
    private TestServer _testServer;

    [TestInitialize]
    public void Inilialize()
    {
        var config = GetConfigurationBuilder();
        var builder = new WebHostBuilder().UseStartup<Startup>().UseConfiguration(config);
        _testServer = new(builder);
    }

    [TestMethod]
    public async Task GetAppInfo_Okay()
    {
        AppInfo expectedAppInfo = new()
        {
            BuildDate = "2024-10-01",
            VersionNumber = "1.0.11"
        };

        HttpClient client = _testServer.CreateClient();

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

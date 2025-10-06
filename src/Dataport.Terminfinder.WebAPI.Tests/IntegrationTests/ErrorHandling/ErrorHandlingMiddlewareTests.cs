using Dataport.Terminfinder.WebAPI.ErrorHandling;

namespace Dataport.Terminfinder.WebAPI.Tests.IntegrationTests.ErrorHandling;

[TestClass]
[TestCategory("Integrationtest")]
public class ErrorHandlingMiddlewareTests : BaseIntegrationTests
{
    private TestServer _server;
    private HttpClient _client;

    [TestInitialize]
    public void Setup()
    {
        var mockLocalizer = new Mock<IStringLocalizer<ErrorMessageResources>>();

        var config = GetConfigurationBuilder();
        var builder = new WebHostBuilder()
            .UseStartup<Startup>()
            .UseConfiguration(config)
            .ConfigureServices(services =>
            {
                services.AddSingleton(mockLocalizer.Object);
            })
            .Configure(app =>
            {
                app.UseMiddleware<ErrorHandlingMiddleware>();
                app.Map("/conflict", cfg =>
                {
                    cfg.Run(_ => throw new ConflictException(ErrorType.GeneralError));
                });
            });

        _server = new TestServer(builder);
        _client = _server.CreateClient();
    }

    [TestMethod]
    public async Task Conflict_ReturnsExpectedException()
    {
        var response = await _client.GetAsync("conflict");
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.Conflict, response.StatusCode);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _client.Dispose();
        _server.Dispose();
    }
}
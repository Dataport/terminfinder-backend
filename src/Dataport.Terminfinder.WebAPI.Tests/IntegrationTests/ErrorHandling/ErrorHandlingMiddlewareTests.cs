using Dataport.Terminfinder.WebAPI.ErrorHandling;

namespace Dataport.Terminfinder.WebAPI.Tests.IntegrationTests.ErrorHandling;

[TestClass]
[TestCategory("Integrationtest")]
public class ErrorHandlingMiddlewareTests : BaseIntegrationTests
{
    private TestServer _server;
    private IHost _host;
    private HttpClient _client;

    [TestInitialize]
    public async Task Setup()
    {
        var mockLocalizer = new Mock<IStringLocalizer<ErrorMessageResources>>();

        var config = GetConfigurationBuilder();
        _host = new HostBuilder()
            .ConfigureWebHost(builder =>
                {
                    builder
                        .UseTestServer()
                        .UseStartup<Startup>()
                        .UseConfiguration(config)
                        .ConfigureServices(services =>
                            {
                                services.AddSingleton(mockLocalizer.Object);
                            }
                        )
                        .Configure(app =>
                            {
                                app.UseMiddleware<ErrorHandlingMiddleware>();
                                app.Map(
                                    "/conflict",
                                    cfg =>
                                    {
                                        cfg.Run(_ => throw new ConflictException(ErrorType.GeneralError));
                                    }
                                );
                            }
                        );
                }
            )
            .Build();

        await _host.StartAsync();
        _server = _host.GetTestServer();
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
    public async Task Cleanup()
    {
        if (_server != null)
        {
            await _host.StopAsync();
            _server.Dispose();
        }

        _host?.Dispose();
        _client.Dispose();
    }
}
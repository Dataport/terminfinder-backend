namespace Dataport.Terminfinder.WebAPI.Tests.IntegrationTests;

[TestClass]
[TestCategory("Integrationtest")]
public class CustomerControllerIntegrationTests : BaseIntegrationTests
{
    private TestServer _testServer;
    private IHost _host;
    private static readonly Guid ExpectedCustomerId = new("E1E81104-3944-4588-A48E-B64BDE473E1A");

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
    public async Task GetCustomer_Okay()
    {
        var expectedCustomer = new Customer
        {
            CustomerId = ExpectedCustomerId,
            CustomerName = "Test",
            Status = AppointmentStatusType.Started.ToString()
        };

        var client = _testServer.CreateClient();

        // Act
        var response = await client.GetAsync($"customer/{expectedCustomer.CustomerId}");
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        var responseText = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<Customer>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(dto);
        Assert.IsInstanceOfType(dto, typeof(Customer));
        Assert.AreEqual(expectedCustomer.CustomerId, dto.CustomerId);
        Assert.AreEqual(expectedCustomer.CustomerName, dto.CustomerName);
        Assert.AreEqual(expectedCustomer.Status, dto.Status);
    }

    [TestMethod]
    public async Task GetCustomer_NotFound()
    {
        var customerId = new Guid("B30FC6C3-5E3A-42CE-BC32-E85F8231992C");

        var client = _testServer.CreateClient();

        // Act
        var response = await client.GetAsync($"customer/{customerId}");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task GetCustomer_NoValidCustomerId_BadRequest()
    {
        var customerId = "12345";

        var client = _testServer.CreateClient();

        // Act
        var response = await client.GetAsync($"customer/{customerId}");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
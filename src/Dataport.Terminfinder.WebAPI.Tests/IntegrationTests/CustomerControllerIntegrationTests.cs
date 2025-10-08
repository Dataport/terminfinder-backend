namespace Dataport.Terminfinder.WebAPI.Tests.IntegrationTests;

[TestClass]
[TestCategory("Integrationtest")]
public class CustomerControllerIntegrationTests : BaseIntegrationTests
{
    private TestServer _testServer;
    private Guid _customerId = new("E1E81104-3944-4588-A48E-B64BDE473E1A");

    [TestInitialize]
    public void Initialize()
    {
        var config = GetConfigurationBuilder();
        var builder = new WebHostBuilder().UseStartup<Startup>().UseConfiguration(config);
        _testServer = new(builder);
    }

    [TestMethod]
    public async Task GetCustomer_Okay()
    {
        Customer expectedCustomer = new()
        {
            CustomerId = _customerId,
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
        Guid customerId = new("B30FC6C3-5E3A-42CE-BC32-E85F8231992C");

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
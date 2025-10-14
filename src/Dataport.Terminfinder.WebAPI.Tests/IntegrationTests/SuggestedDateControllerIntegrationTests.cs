namespace Dataport.Terminfinder.WebAPI.Tests.IntegrationTests;

[TestClass]
[TestCategory("Integrationtest")]
public class SuggestedDateControllerIntegrationTests : BaseIntegrationTests
{
    private TestServer _testServer;
    private static readonly Guid ExpectedCustomerId = new("E1E81104-3944-4588-A48E-B64BDE473E1A");

    [TestInitialize]
    public void Initialize()
    {
        var config = GetConfigurationBuilder();
        var builder = new WebHostBuilder().UseStartup<Startup>().UseConfiguration(config);
        _testServer = new TestServer(builder);
    }

    [TestMethod]
    public async Task DeleteSuggestedDates_Okay()
    {
        var client = _testServer.CreateClient();
        var dto = await CreateTestAppointmentInDatabase(client, ExpectedCustomerId);

        //--- get the appointment
        var appointmentId = dto.AppointmentId;

        // Act
        var response = await client.GetAsync($"appointment/{ExpectedCustomerId}/{appointmentId}");
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        var responseText = await response.Content.ReadAsStringAsync();
        var appointmentResult = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(appointmentResult);
        Assert.IsInstanceOfType(appointmentResult, typeof(Appointment));
        Assert.AreEqual(2, appointmentResult.SuggestedDates.Count);

        // Act
        var result = await client.DeleteAsync($"suggesteddate/{ExpectedCustomerId}/{appointmentId}/{appointmentResult.SuggestedDates.First().SuggestedDateId}");
        result.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

        // Act
        response = await client.GetAsync($"appointment/{ExpectedCustomerId}/{appointmentId}");
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        responseText = await response.Content.ReadAsStringAsync();
        appointmentResult = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(appointmentResult);
        Assert.IsInstanceOfType(appointmentResult, typeof(Appointment));
        Assert.AreEqual(1, appointmentResult.SuggestedDates.Count);
    }

    [TestMethod]
    public async Task DeleteSuggestedDates_SuggestedDates_NotFound()
    {
        var client = _testServer.CreateClient();
        var dto = await CreateTestAppointmentInDatabase(client, ExpectedCustomerId);

        //--- get the appointment
        var appointmentId = dto.AppointmentId;

        // Act
        var result = await client.DeleteAsync($"suggesteddate/{ExpectedCustomerId}/{appointmentId}/{Guid.NewGuid()}");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public async Task DeleteSuggestedDates_AppointmentIsPasswordProtectedNoPasswordSubmitted_Unauthorized()
    {
        var password = "P@$$w0rd";

        var client = _testServer.CreateClient();
        var dto = await CreateTestAppointmentInDatabase(client, ExpectedCustomerId, password);

        //--- get the appointment
        var appointmentId = dto.AppointmentId;

        // Act
        var result = await client.DeleteAsync($"suggesteddate/{ExpectedCustomerId}/{appointmentId}/{dto.SuggestedDates.First().SuggestedDateId}");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [TestMethod]
    public async Task AddSuggestedDateDescription_GetSuggestedDate_ValueMatches()
    {
        // arrange
        const string description = "Im the test description!";
        var client = _testServer.CreateClient();
        
        var appointment = CreateTestAppointment(ExpectedCustomerId, It.IsAny<Guid>());
        appointment.SuggestedDates.First().Description = description;

        // act
        var result = await CreateTestAppointmentInDatabase(client, ExpectedCustomerId, null, appointment);

        // assert
        var resultDescription = result.SuggestedDates.First(s => s.Description==description).Description;
        Assert.IsNotNull(resultDescription);
        Assert.AreEqual(resultDescription, description);
    }
}

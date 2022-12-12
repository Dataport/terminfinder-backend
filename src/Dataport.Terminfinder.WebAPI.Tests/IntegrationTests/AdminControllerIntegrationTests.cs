using System.Net.Http.Headers;
using System.Text;

namespace Dataport.Terminfinder.WebAPI.Tests.IntegrationTests;

[TestClass]
[TestCategory("Integrationtest")]
[ExcludeFromCodeCoverage]
public class AdminControllerIntegrationTests : BaseIntegrationTests
{
    private TestServer _testServer;
    private Guid _customerId = new("E1E81104-3944-4588-A48E-B64BDE473E1A");

    [TestInitialize]
    public void Inilialize()
    {
        var config = GetConfigurationBuilder();
        var builder = new WebHostBuilder().UseStartup<Startup>().UseConfiguration(config);
        _testServer = new(builder);
    }

    [TestMethod]
    public async Task GetApppointment()
    {
        HttpClient client = _testServer.CreateClient();
        var dto = await CreateTestAppointmentInDatabase(client, _customerId);

        //--- get the appointment
        var adminId = dto.AdminId;

        // Act
        var response = await client.GetAsync($"admin/{_customerId}/{adminId}");
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        var responseText = await response.Content.ReadAsStringAsync(); 
        var appointmentResult = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(appointmentResult);
        Assert.IsInstanceOfType(appointmentResult, typeof(Appointment));

        // Act protection
        var responseProtection = await client.GetAsync($"admin/{_customerId}/{adminId}/protection");
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(responseProtection);
        responseText = await responseProtection.Content.ReadAsStringAsync();
        var appointmentProtectionResult = JsonConvert.DeserializeObject<AppointmentProtectionResult>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(appointmentProtectionResult);
        Assert.IsInstanceOfType(appointmentProtectionResult, typeof(AppointmentProtectionResult));
        Assert.AreEqual(false, appointmentProtectionResult.IsProtectedByPassword);
    }

    [TestMethod]
    public async Task GetApppointment_AdminIdIsEmpty()
    {
        HttpClient client = _testServer.CreateClient();
        await CreateTestAppointmentInDatabase(client, _customerId);

        //--- get the appointment
        var adminId = Guid.Empty;

        // Act
        var response = await client.GetAsync($"admin/{_customerId}/{adminId}");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task GetApppointment_NotFound()
    {
        HttpClient client = _testServer.CreateClient();
        await CreateTestAppointmentInDatabase(client, _customerId);

        //--- get the appointment
        var adminId = Guid.NewGuid();

        // Act
        var response = await client.GetAsync($"admin/{_customerId}/{adminId}");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task GetApppointment_verificationFailed_Unauthorized()
    {
        var password = "P@$$w0rd";

        HttpClient client = _testServer.CreateClient();
        var dto = await CreateTestAppointmentInDatabase(client, _customerId, password);

        //--- get the appointment
        var adminId = dto.AdminId;

        // Act
        var response = await client.GetAsync($"admin/{_customerId}/{adminId}");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);

        // Act Protection
        var responseProtection = await client.GetAsync($"admin/{_customerId}/{adminId}/protection");
        responseProtection.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(responseProtection);
        var responseText = await responseProtection.Content.ReadAsStringAsync();
        var appointmentProtectionResult = JsonConvert.DeserializeObject<AppointmentProtectionResult>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, responseProtection.StatusCode);
        Assert.IsNotNull(appointmentProtectionResult);
        Assert.IsInstanceOfType(appointmentProtectionResult, typeof(AppointmentProtectionResult));
        Assert.AreEqual(true, appointmentProtectionResult.IsProtectedByPassword);
    }

    [TestMethod]
    public async Task GetApppointment_verificationSuccessful_okay()
    {
        var password = "P@$$w0rd";

        HttpClient client = _testServer.CreateClient();
        var dto = await CreateTestAppointmentInDatabase(client, _customerId, password);
        var adminId = dto.AdminId;

        // Act Protection
        var responseProtection = await client.GetAsync($"admin/{_customerId}/{adminId}/protection");
        responseProtection.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(responseProtection);
        var responseText = await responseProtection.Content.ReadAsStringAsync();
        var appointmentProtectionResult = JsonConvert.DeserializeObject<AppointmentProtectionResult>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, responseProtection.StatusCode);
        Assert.IsNotNull(appointmentProtectionResult);
        Assert.IsInstanceOfType(appointmentProtectionResult, typeof(AppointmentProtectionResult));
        Assert.AreEqual(true, appointmentProtectionResult.IsProtectedByPassword);

        //--- get the appointment
        // Act
        var credential = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{adminId}:{password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credential);
        var response = await client.GetAsync($"admin/{_customerId}/{adminId}");

        // Assert
        Assert.IsNotNull(response);
        responseText = await response.Content.ReadAsStringAsync();
        var appointmentResult = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(appointmentResult);
        Assert.IsInstanceOfType(appointmentResult, typeof(Appointment));
    }

    [TestMethod]
    public async Task SetStatus_appointmentStatusTypeStarted_Okay()
    {
        HttpClient client = _testServer.CreateClient();
        var dto = await CreateTestAppointmentInDatabase(client, _customerId);

        //--- get the appointment
        var adminId = dto.AdminId;

        // Act
        var response = await client.PutAsync($"admin/{_customerId}/{adminId}/{AppointmentStatusType.Paused}/status", null);
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        var responseText = await response.Content.ReadAsStringAsync();
        var appointmentResult = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(appointmentResult);
        Assert.IsInstanceOfType(appointmentResult, typeof(Appointment));

        // Act
        response = await client.GetAsync($"admin/{_customerId}/{adminId}");
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        responseText = await response.Content.ReadAsStringAsync();
        appointmentResult = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(appointmentResult);
        Assert.IsInstanceOfType(appointmentResult, typeof(Appointment));
        Assert.AreEqual(AppointmentStatusType.Paused, appointmentResult.AppointmentStatus);
    }

    [TestMethod]
    public async Task GetPasswordVerifcation_verificationSuccessful_True()
    {
        var password = "P@$$w0rd";

        HttpClient client = _testServer.CreateClient();
        var dto = await CreateTestAppointmentInDatabase(client, _customerId, password);
        var adminId = dto.AdminId;

        // Act Protection
        var credential = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{adminId}:{password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credential);
        var responseProtection = await client.GetAsync($"admin/{_customerId}/{adminId}/passwordverification");
        responseProtection.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(responseProtection);
        var responseText = await responseProtection.Content.ReadAsStringAsync();
        var appointmentPasswordVerificationResult = JsonConvert.DeserializeObject<AppointmentPasswordVerificationResult>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, responseProtection.StatusCode);
        Assert.IsNotNull(appointmentPasswordVerificationResult);
        Assert.IsInstanceOfType(appointmentPasswordVerificationResult, typeof(AppointmentPasswordVerificationResult));
        Assert.AreEqual(true, appointmentPasswordVerificationResult.IsProtectedByPassword);
        Assert.AreEqual(true, appointmentPasswordVerificationResult.IsPasswordValid);
    }

    [TestMethod]
    public async Task GetPasswordVerifcation_verificationSuccessful_False()
    {
        var password = "P@$$w0rd";

        HttpClient client = _testServer.CreateClient();
        var dto = await CreateTestAppointmentInDatabase(client, _customerId, password);
        var adminId = dto.AdminId;

        // Act Protection
        var credential = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{adminId}:x2er4A-4"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credential);
        var responseProtection = await client.GetAsync($"admin/{_customerId}/{adminId}/passwordverification");
        responseProtection.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(responseProtection);
        var responseText = await responseProtection.Content.ReadAsStringAsync();
        var appointmentPasswordVerificationResult = JsonConvert.DeserializeObject<AppointmentPasswordVerificationResult>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, responseProtection.StatusCode);
        Assert.IsNotNull(appointmentPasswordVerificationResult);
        Assert.IsInstanceOfType(appointmentPasswordVerificationResult, typeof(AppointmentPasswordVerificationResult));
        Assert.AreEqual(true, appointmentPasswordVerificationResult.IsProtectedByPassword);
        Assert.AreEqual(false, appointmentPasswordVerificationResult.IsPasswordValid);
    }

    [TestMethod]
    public async Task GetPasswordVerifcation_AppointmentNotProtected_True()
    {
        var password = "P@$$w0rd";

        HttpClient client = _testServer.CreateClient();
        var dto = await CreateTestAppointmentInDatabase(client, _customerId);
        var adminId = dto.AdminId;

        // Act Protection
        var credential = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{adminId}:{password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credential);
        var responseProtection = await client.GetAsync($"admin/{_customerId}/{adminId}/passwordverification");
        responseProtection.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(responseProtection);
        var responseText = await responseProtection.Content.ReadAsStringAsync();
        var appointmentPasswordVerificationResult = JsonConvert.DeserializeObject<AppointmentPasswordVerificationResult>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, responseProtection.StatusCode);
        Assert.IsNotNull(appointmentPasswordVerificationResult);
        Assert.IsInstanceOfType(appointmentPasswordVerificationResult, typeof(AppointmentPasswordVerificationResult));
        Assert.AreEqual(false, appointmentPasswordVerificationResult.IsProtectedByPassword);
        Assert.AreEqual(false, appointmentPasswordVerificationResult.IsPasswordValid);
    }
}
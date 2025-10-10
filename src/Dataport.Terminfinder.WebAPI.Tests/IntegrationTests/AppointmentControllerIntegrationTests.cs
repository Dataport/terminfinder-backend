using Dataport.Terminfinder.WebAPI.Constants;
using System.Net.Http.Headers;
using System.Text;

namespace Dataport.Terminfinder.WebAPI.Tests.IntegrationTests;

[TestClass]
[TestCategory("Integrationtest")]
public class AppointmentControllerIntegrationTests : BaseIntegrationTests
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
    public async Task AddAppointment()
    {
        var appointment = CreateTestAppointment(_customerId, Guid.Empty);

        var client = _testServer.CreateClient();

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8, HttpConstants.TerminfinderMediaTypeJsonV1);

        var response = await client.PostAsync($"appointment/{_customerId}", content);
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        var responseText = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(dto);
        Assert.IsInstanceOfType(dto, typeof(Appointment));
        Assert.IsNotNull(dto.AppointmentId);

        //--- get the appointment
        var appointmentId= dto.AppointmentId;

        // Act
        response = await client.GetAsync($"appointment/{_customerId}/{appointmentId}");
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        responseText = await response.Content.ReadAsStringAsync();
        dto = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(dto);
        Assert.IsInstanceOfType(dto, typeof(Appointment));
        Assert.AreEqual(appointmentId, dto.AppointmentId);
        Assert.AreEqual(appointment.CreatorName, dto.CreatorName);
        Assert.AreEqual(appointment.Subject, dto.Subject);
        Assert.AreEqual(appointment.Description, dto.Description);
        Assert.AreEqual(appointment.Place, dto.Place);
        Assert.AreEqual(appointment.AppointmentStatus, dto.AppointmentStatus);
        Assert.AreEqual(appointment.SuggestedDates.Count, dto.SuggestedDates.Count);

        var responseSuggestedDates = dto.SuggestedDates.OrderBy(s => s.StartDate).ToList();
        var expectedSuggestedDates = appointment.SuggestedDates.OrderBy(s => s.StartDate).ToList();

        Assert.AreEqual(expectedSuggestedDates[0].StartDate.Date, responseSuggestedDates[0].StartDate.Date);

        Assert.AreEqual(((DateTime)expectedSuggestedDates[0].EndDate!).Date, ((DateTime)responseSuggestedDates[0].EndDate!).Date);
        Assert.IsNull(responseSuggestedDates[0].StartTime);
        Assert.IsNull(responseSuggestedDates[0].EndTime);
        Assert.AreEqual(expectedSuggestedDates[1].StartDate.Date, responseSuggestedDates[1].StartDate.Date);
        Assert.AreEqual(((DateTime)expectedSuggestedDates[1].EndDate!).Date, ((DateTime)responseSuggestedDates[1].EndDate!).Date);

        var timeExpectedUtf = (expectedSuggestedDates[1].StartTime ?? default).ToUniversalTime();
        var timeResponseUtf = (responseSuggestedDates[1].StartTime ?? default).ToUniversalTime();
        Assert.AreEqual(timeExpectedUtf.Hour, timeResponseUtf.Hour);
        Assert.AreEqual(timeExpectedUtf.Minute, timeResponseUtf.Minute);

        timeExpectedUtf = (expectedSuggestedDates[1].EndTime ?? default).ToUniversalTime();
        timeResponseUtf = (responseSuggestedDates[1].EndTime ?? default).ToUniversalTime();
        Assert.AreEqual(timeExpectedUtf.Hour, timeResponseUtf.Hour);
        Assert.AreEqual(timeExpectedUtf.Minute, timeResponseUtf.Minute);
        Assert.AreEqual(((DateTime)expectedSuggestedDates[1].EndDate).Date, ((DateTime)responseSuggestedDates[1].EndDate).Date);

        //-- Get protection
        // Act
        response = await client.GetAsync($"appointment/{_customerId}/{appointmentId}/protection");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        responseText = await response.Content.ReadAsStringAsync();
        var protectionResult = JsonConvert.DeserializeObject<AppointmentProtectionResult>(responseText);
        Assert.IsNotNull(protectionResult);
        Assert.IsInstanceOfType(protectionResult, typeof(AppointmentProtectionResult));
        Assert.AreEqual(appointmentId, protectionResult.AppointmentId);
        Assert.IsFalse(protectionResult.IsProtectedByPassword);

        //-- get GetPasswordVerification
        // Act
        response = await client.GetAsync($"appointment/{_customerId}/{appointmentId}/passwordverification");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        responseText = await response.Content.ReadAsStringAsync();
        var passwordVerificationResult = JsonConvert.DeserializeObject<AppointmentPasswordVerificationResult>(responseText);
        Assert.IsNotNull(passwordVerificationResult);
        Assert.IsInstanceOfType(passwordVerificationResult, typeof(AppointmentPasswordVerificationResult));
        Assert.AreEqual(appointmentId, passwordVerificationResult.AppointmentId);
        Assert.IsFalse(passwordVerificationResult.IsProtectedByPassword);
        Assert.IsFalse(passwordVerificationResult.IsPasswordValid);
    }

    [TestMethod]
    public async Task UpdateAppointment()
    {
        var appointment = CreateTestAppointment(_customerId, Guid.Empty);

        var client = _testServer.CreateClient();

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8, HttpConstants.TerminfinderMediaTypeJsonV1);

        var response = await client.PostAsync($"appointment/{_customerId}", content);
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        var responseText = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(dto);
        Assert.IsInstanceOfType(dto, typeof(Appointment));
        Assert.IsNotNull(dto.AppointmentId);

        var appointmentId = dto.AppointmentId;
        var adminId = dto.AdminId;

        //--- get the appointment
        // Act
        response = await client.GetAsync($"appointment/{_customerId}/{appointmentId}");
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        responseText = await response.Content.ReadAsStringAsync();
        dto = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(dto);
        Assert.IsInstanceOfType(dto, typeof(Appointment));

        //-- Update
        dto.CreatorName = "Mike";
        dto.Subject = "new appointment";
        dto.Description = "new description";

        //new suggestedDate
        SuggestedDate suggestedDate3 = new()
        {
            AppointmentId = Guid.Empty,
            CustomerId = _customerId,
            SuggestedDateId = Guid.Empty,
            StartDate = DateTime.Now.AddDays(3),
            EndDate = DateTime.Now.AddDays(4)
        };
        dto.SuggestedDates.Add(suggestedDate3);

        // Act
        content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, HttpConstants.TerminfinderMediaTypeJsonV1);
        response = await client.PutAsync($"appointment/{_customerId}", content);

        // Assert -- badrequest, no adminId
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        // Act
        dto.AdminId = adminId;
        content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, HttpConstants.TerminfinderMediaTypeJsonV1);
        response = await client.PutAsync($"appointment/{_customerId}", content);

        // Assert
        Assert.IsNotNull(response);
        responseText = await response.Content.ReadAsStringAsync();
        var dto3 = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(dto3);
        Assert.IsInstanceOfType(dto3, typeof(Appointment));
        Assert.IsNotNull(dto3.AppointmentId);
        Assert.AreEqual(dto.AppointmentId, dto3.AppointmentId);
        Assert.AreEqual(dto.AdminId, dto3.AdminId);

        appointmentId = dto3.AppointmentId;

        //--- get the appointment
        // Act
        response = await client.GetAsync($"appointment/{_customerId}/{appointmentId}");
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        responseText = await response.Content.ReadAsStringAsync();
        var dto2 = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(dto2);
        Assert.IsInstanceOfType(dto2, typeof(Appointment));
        Assert.AreEqual(appointmentId, dto2.AppointmentId);
        Assert.AreEqual(dto.CreatorName, dto2.CreatorName);
        Assert.AreEqual(dto.Subject, dto2.Subject);
        Assert.AreEqual(dto.Description, dto2.Description);
        Assert.AreEqual(dto.Place, dto2.Place);
        Assert.AreEqual(dto.AppointmentStatus, dto2.AppointmentStatus);
        Assert.AreEqual(3, dto2.SuggestedDates.Count);

        var responseSuggestedDates = dto2.SuggestedDates.OrderBy(s => s.StartDate).ToList();
        var expectedSuggestedDates = dto.SuggestedDates.OrderBy(s => s.StartDate).ToList();

        Assert.AreEqual(expectedSuggestedDates[0].StartDate.Date, responseSuggestedDates[0].StartDate.Date);

        Assert.AreEqual(((DateTime)expectedSuggestedDates[0].EndDate!).Date, ((DateTime)responseSuggestedDates[0].EndDate!).Date);
        Assert.IsNull(responseSuggestedDates[0].StartTime);
        Assert.IsNull(responseSuggestedDates[0].EndTime);

        Assert.AreEqual(expectedSuggestedDates[1].StartDate.Date, responseSuggestedDates[1].StartDate.Date);
        Assert.AreEqual(((DateTime)expectedSuggestedDates[1].EndDate!).Date, ((DateTime)responseSuggestedDates[1].EndDate!).Date);

        var timeExpectedUtf = (expectedSuggestedDates[1].StartTime ?? default).ToUniversalTime();
        var timeResponseUtf = (responseSuggestedDates[1].StartTime ?? default).ToUniversalTime();
        Assert.AreEqual(timeExpectedUtf.Hour, timeResponseUtf.Hour);
        Assert.AreEqual(timeExpectedUtf.Minute, timeResponseUtf.Minute);

        timeExpectedUtf = (expectedSuggestedDates[1].EndTime ?? default).ToUniversalTime();
        timeResponseUtf = (responseSuggestedDates[1].EndTime ?? default).ToUniversalTime();
        Assert.AreEqual(timeExpectedUtf.Hour, timeResponseUtf.Hour);
        Assert.AreEqual(timeExpectedUtf.Minute, timeResponseUtf.Minute);
        Assert.AreEqual(((DateTime)expectedSuggestedDates[1].EndDate).Date, ((DateTime)responseSuggestedDates[1].EndDate).Date);

        Assert.AreEqual(expectedSuggestedDates[2].StartDate.Date, responseSuggestedDates[2].StartDate.Date);
        Assert.AreEqual(((DateTime)expectedSuggestedDates[2].EndDate!).Date, ((DateTime)responseSuggestedDates[2].EndDate!).Date);
        Assert.IsNull(responseSuggestedDates[2].StartTime);
        Assert.IsNull(responseSuggestedDates[2].EndTime);
    }

    [TestMethod]
    public async Task RepostAppointment_SameObject_IsValid()
    {
        // Arrange
        var appointment = CreateTestAppointment(_customerId, Guid.Empty);
        var client = _testServer.CreateClient();

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8,
            HttpConstants.TerminfinderMediaTypeJsonV1);
        var response = await client.PostAsync($"appointment/{_customerId}", content);

        // Assert
        Assert.IsNotNull(response);
        response.EnsureSuccessStatusCode();

        // Act
        var responseText = await response.Content.ReadAsStringAsync();
        var appointmentResult = JsonConvert.DeserializeObject<Appointment>(responseText);

        // resend previous result
        var contentRepost = new StringContent(JsonConvert.SerializeObject(appointmentResult), Encoding.UTF8,
            HttpConstants.TerminfinderMediaTypeJsonV1);
        var responseRepost = await client.PostAsync($"appointment/{_customerId}", contentRepost);

        // Assert
        Assert.IsNotNull(responseRepost);
        responseRepost.EnsureSuccessStatusCode();

        var responseRepostText = await responseRepost.Content.ReadAsStringAsync();
        Assert.AreEqual(responseText, responseRepostText);
    }

    [TestMethod]
    public async Task RepostAppointment_DifferentObject_PersistentValuesUnchanged()
    {
        // Arrange
        var appointment = CreateTestAppointment(_customerId, Guid.Empty);
        var client = _testServer.CreateClient();

        // Act
        var contentString = JsonConvert.SerializeObject(appointment);
        var content = new StringContent(contentString, Encoding.UTF8, HttpConstants.TerminfinderMediaTypeJsonV1);
        var response = await client.PostAsync($"appointment/{_customerId}", content);

        // Assert
        Assert.IsNotNull(response);
        response.EnsureSuccessStatusCode();

        // resend previous result
        // Act
        var responseText = await response.Content.ReadAsStringAsync();
        var appointmentResponse = JsonConvert.DeserializeObject<Appointment>(responseText);
        var responseVerify1 = await client.GetAsync($"appointment/{_customerId}/{appointmentResponse.AppointmentId}");

        appointmentResponse.CreatorName = "New Creator!";

        var contentRepost = new StringContent(JsonConvert.SerializeObject(appointmentResponse), Encoding.UTF8,
            HttpConstants.TerminfinderMediaTypeJsonV1);
        var responseRepost = await client.PostAsync($"appointment/{_customerId}", contentRepost);

        // Assert
        Assert.IsNotNull(responseRepost);
        responseRepost.EnsureSuccessStatusCode();

        var responseRepostText = await responseRepost.Content.ReadAsStringAsync();
        Assert.AreNotEqual(responseText, responseRepostText);

        // verify that values are unchanged from second post request
        // Act
        var responseVerify2 = await client.GetAsync($"appointment/{_customerId}/{appointmentResponse.AppointmentId}");
        var responseVerify1Text = await responseVerify1.Content.ReadAsStringAsync();
        var responseVerify2Text = await responseVerify2.Content.ReadAsStringAsync();

        // Assert
        Assert.IsNotNull(responseVerify1);
        Assert.IsNotNull(responseVerify2);
        responseVerify1.EnsureSuccessStatusCode();
        responseVerify2.EnsureSuccessStatusCode();

        Assert.AreEqual(responseVerify1Text, responseVerify2Text);
    }

    [TestMethod]
    public async Task GetAppointment_AppointmentId_NotFound()
    {
        Guid appointmentId = new("C1C2474B-488A-4ECF-94E8-47387BB715D5");

        var client = _testServer.CreateClient();

        // Act
        var response = await client.GetAsync($"appointment/{_customerId}/{appointmentId}");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task GetAppointment_AppointmentId_InvalidGuid()
    {
        var appointmentId = "C1C2474B-488A-4ECF-94E8-47387BB715D6";

        var client = _testServer.CreateClient();

        // Act
        var response = await client.GetAsync($"appointment/{_customerId}/{appointmentId}");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task GetAppointment_AppointmentId_InvalidCustomerId()
    {
        var appointmentId = "C1C2474B-488A-4ECF-94E8-47387BB715D5";

        var client = _testServer.CreateClient();

        // Act
        var response = await client.GetAsync($"appointment/{_customerId}/{appointmentId}");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task AddAppointment_WithAdminId_BadRequest()
    {
        Guid adminId = new("FFFD657A-4D06-40DB-8443-D67BBB950EE7");

        var appointment = CreateTestAppointment(_customerId, adminId);

        var client = _testServer.CreateClient();

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8,
            HttpConstants.TerminfinderMediaTypeJsonV1);

        var response = await client.PostAsync($"appointment/{_customerId}", content);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task AddAppointment_WithPassword_Unauthorized()
    {
        var appointment = CreateTestAppointment(_customerId, Guid.Empty, "P@$$w0rd");

        var client = _testServer.CreateClient();

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8, HttpConstants.TerminfinderMediaTypeJsonV1);

        var response = await client.PostAsync($"appointment/{_customerId}", content);

        // Assert
        Assert.IsNotNull(response);
        var responseText = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(dto);
        Assert.IsInstanceOfType(dto, typeof(Appointment));
        Assert.IsNotNull(dto.AppointmentId);

        //--- get the appointment
        var appointmentId = dto.AppointmentId;

        // Act
        response = await client.GetAsync($"appointment/{_customerId}/{appointmentId}");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task AddAppointment_WithWrongPassword_BadRequest()
    {
        //--- too short
        var password = "12345";

        var appointment = CreateTestAppointment(_customerId, Guid.Empty, password);

        var client = _testServer.CreateClient();

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8, HttpConstants.TerminfinderMediaTypeJsonV1);
        var response = await client.PostAsync($"appointment/{_customerId}", content);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        //--- only digits
        password = "12345678";
        appointment.Password = password;

        content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8, HttpConstants.TerminfinderMediaTypeJsonV1);
        response = await client.PostAsync($"appointment/{_customerId}", content);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        //--- too long
        password = "A-e1234567890909090k0990lk09l8900lk00kk";
        appointment.Password = password;

        content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8, HttpConstants.TerminfinderMediaTypeJsonV1);
        response = await client.PostAsync($"appointment/{_customerId}", content);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task AddAppointment_WithPassword()
    {
        var password = "P@$$w0rd";

        var appointment = CreateTestAppointment(_customerId, Guid.Empty, password);

        var client = _testServer.CreateClient();

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8, HttpConstants.TerminfinderMediaTypeJsonV1);

        var response = await client.PostAsync($"appointment/{_customerId}", content);

        // Assert
        Assert.IsNotNull(response);
        var responseText = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(dto);
        Assert.IsInstanceOfType(dto, typeof(Appointment));
        Assert.IsNotNull(dto.AppointmentId);
        Assert.IsNotNull(dto.AdminId);

        //--- get the appointment
        var appointmentId = dto.AppointmentId;

        // Act - wrong password
        var credential = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{appointmentId}:23rfgt4-We"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credential);
        response = await client.GetAsync($"appointment/{_customerId}/{appointmentId}");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);

        // Act
        credential = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{appointmentId}:{password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credential);
        response = await client.GetAsync($"appointment/{_customerId}/{appointmentId}");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        responseText = await response.Content.ReadAsStringAsync();
        dto = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.IsNotNull(dto);
        Assert.IsInstanceOfType(dto, typeof(Appointment));
        Assert.AreEqual(Guid.Empty, dto.AdminId);
        Assert.AreEqual(appointmentId, dto.AppointmentId);
        Assert.AreEqual(appointment.CreatorName, dto.CreatorName);
        Assert.AreEqual(appointment.Subject, dto.Subject);
        Assert.AreEqual(appointment.Description, dto.Description);
        Assert.AreEqual(appointment.Place, dto.Place);
        Assert.AreEqual(appointment.AppointmentStatus, dto.AppointmentStatus);
    }

    [TestMethod]
    public async Task GetProtection_appointmentExistsAndAppointmentIsProtectedByPassword_Okay()
    {
        var password = "P@$$w0rd";

        var appointment = CreateTestAppointment(_customerId, Guid.Empty, password);

        var client = _testServer.CreateClient();

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8,
            HttpConstants.TerminfinderMediaTypeJsonV1);

        var response = await client.PostAsync($"appointment/{_customerId}", content);

        // Assert
        Assert.IsNotNull(response);
        var responseText = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(dto);
        Assert.IsInstanceOfType(dto, typeof(Appointment));
        Assert.IsNotNull(dto.AppointmentId);
        Assert.IsNotNull(dto.AdminId);
        var appointmentId = dto.AppointmentId;


        // Act
        response = await client.GetAsync($"appointment/{_customerId}/{appointmentId}/protection");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        responseText = await response.Content.ReadAsStringAsync();
        var protectionResult = JsonConvert.DeserializeObject<AppointmentProtectionResult>(responseText);
        Assert.IsNotNull(protectionResult);
        Assert.IsInstanceOfType(protectionResult, typeof(AppointmentProtectionResult));
        Assert.AreEqual(appointmentId, protectionResult.AppointmentId);
        Assert.IsTrue(protectionResult.IsProtectedByPassword);
    }

    [TestMethod]
    public async Task GetPasswordVerification()
    {
        var password = "P@$$w0rd";

        var appointment = CreateTestAppointment(_customerId, Guid.Empty, password);

        var client = _testServer.CreateClient();

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8,
            HttpConstants.TerminfinderMediaTypeJsonV1);

        var response = await client.PostAsync($"appointment/{_customerId}", content);

        // Assert
        Assert.IsNotNull(response);
        var responseText = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(dto);
        Assert.IsInstanceOfType(dto, typeof(Appointment));
        Assert.IsNotNull(dto.AppointmentId);
        Assert.IsNotNull(dto.AdminId);
        var appointmentId = dto.AppointmentId;
        var adminId = dto.AdminId;

        // Act
        var credential = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{adminId}:{password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credential);
        response = await client.GetAsync($"appointment/{_customerId}/{appointmentId}/passwordverification");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        responseText = await response.Content.ReadAsStringAsync();
        var passwordVerificationResult = JsonConvert.DeserializeObject<AppointmentPasswordVerificationResult>(responseText);
        Assert.IsNotNull(passwordVerificationResult);
        Assert.IsInstanceOfType(passwordVerificationResult, typeof(AppointmentPasswordVerificationResult));
        Assert.AreEqual(appointmentId, passwordVerificationResult.AppointmentId);
        Assert.IsTrue(passwordVerificationResult.IsProtectedByPassword);
        Assert.IsTrue(passwordVerificationResult.IsPasswordValid);

        // Act - password is not valid
        credential = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{adminId}:7873jfe-gteA"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credential);
        response = await client.GetAsync($"appointment/{_customerId}/{appointmentId}/passwordverification");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        responseText = await response.Content.ReadAsStringAsync();
        passwordVerificationResult = JsonConvert.DeserializeObject<AppointmentPasswordVerificationResult>(responseText);
        Assert.IsNotNull(passwordVerificationResult);
        Assert.IsInstanceOfType(passwordVerificationResult, typeof(AppointmentPasswordVerificationResult));
        Assert.AreEqual(appointmentId, passwordVerificationResult.AppointmentId);
        Assert.IsTrue(passwordVerificationResult.IsProtectedByPassword);
        Assert.IsFalse(passwordVerificationResult.IsPasswordValid);
    }
}
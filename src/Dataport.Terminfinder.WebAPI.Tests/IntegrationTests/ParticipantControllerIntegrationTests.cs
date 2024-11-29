using Dataport.Terminfinder.WebAPI.Constants;
using System.Net.Http.Headers;
using System.Text;

namespace Dataport.Terminfinder.WebAPI.Tests.IntegrationTests;

[TestClass]
[TestCategory("Integrationtest")]
[ExcludeFromCodeCoverage]
public class ParticipantControllerIntegrationTests : BaseIntegrationTests
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
    public async Task DeleteParticipants_Okay()
    {
        HttpClient client = _testServer.CreateClient();
        var dto = await CreateTestAppointmentInDatabase(client, _customerId);

        //--- get the appointment
        var appointmentId = dto.AppointmentId;

        //--- add participiant
        Voting voting1 = new()
        {
            VotingId = Guid.Empty,
            CustomerId = _customerId,
            AppointmentId = dto.AppointmentId,
            ParticipantId = Guid.Empty,
            SuggestedDateId = dto.SuggestedDates.First().SuggestedDateId,
            Status = VotingStatusType.Accepted
        };

        var participants = dto.Participants ?? new List<Participant>();
        participants.Add(new()
        {
            AppointmentId = dto.AppointmentId,
            CustomerId = _customerId,
            ParticipantId = Guid.Empty,
            Name = "Joe",
            Votings = new List<Voting>() { voting1 }
        });

        // Act
        var contentParticipant = new StringContent(JsonConvert.SerializeObject(participants), Encoding.UTF8,
            HttpConstants.TerminfinderMediaTypeJsonV1);
        var response = await client.PutAsync($"votings/{_customerId}/{appointmentId}", contentParticipant);
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        var responseText = await response.Content.ReadAsStringAsync();
        var responseparticipant = JsonConvert.DeserializeObject<Participant[]>(responseText);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(responseparticipant);

        // Act
        var responseAppointment = await client.GetAsync($"appointment/{_customerId}/{appointmentId}");
        responseAppointment.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(responseAppointment);
        responseText = await responseAppointment.Content.ReadAsStringAsync();
        var appointmentResult = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, responseAppointment.StatusCode);
        Assert.IsNotNull(appointmentResult);
        Assert.IsInstanceOfType(appointmentResult, typeof(Appointment));
        Assert.AreEqual(2, appointmentResult.SuggestedDates.Count);
        Assert.AreEqual(1, appointmentResult.Participants.Count);

        // Act
        var result = await client.DeleteAsync($"participant/{_customerId}/{appointmentId}/{appointmentResult.Participants.First().ParticipantId}");
        result.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);

        // Act
        responseAppointment = await client.GetAsync($"appointment/{_customerId}/{appointmentId}");
        responseAppointment.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(responseAppointment);
        responseText = await responseAppointment.Content.ReadAsStringAsync();
        appointmentResult = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, responseAppointment.StatusCode);
        Assert.IsNotNull(appointmentResult);
        Assert.IsInstanceOfType(appointmentResult, typeof(Appointment));
        Assert.AreEqual(2, appointmentResult.SuggestedDates.Count);
        Assert.AreEqual(0, appointmentResult.Participants.Count);
    }

    [TestMethod]
    public async Task DeleteParticipants_Participants_NotFound()
    {
        HttpClient client = _testServer.CreateClient();
        var dto = await CreateTestAppointmentInDatabase(client, _customerId);

        //--- get the appointment
        var appointmentId = dto.AppointmentId;

        //--- add participiant
        Voting voting1 = new()
        {
            VotingId = Guid.Empty,
            CustomerId = _customerId,
            AppointmentId = dto.AppointmentId,
            ParticipantId = Guid.Empty,
            SuggestedDateId = dto.SuggestedDates.First().SuggestedDateId,
            Status = VotingStatusType.Accepted
        };

        var participants = dto.Participants ?? new List<Participant>();
        participants.Add(new()
        {
            AppointmentId = dto.AppointmentId,
            CustomerId = _customerId,
            ParticipantId = Guid.Empty,
            Name = "Joe",
            Votings = new List<Voting>() { voting1 }
        });

        // Act
        var contentParticipant = new StringContent(JsonConvert.SerializeObject(participants), Encoding.UTF8,
            HttpConstants.TerminfinderMediaTypeJsonV1);
        var response = await client.PutAsync($"votings/{_customerId}/{appointmentId}", contentParticipant);
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        var responseText = await response.Content.ReadAsStringAsync();
        var responseparticipant = JsonConvert.DeserializeObject<Participant[]>(responseText);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(responseparticipant);

        // Act
        var responseAppointment = await client.GetAsync($"appointment/{_customerId}/{appointmentId}");
        responseAppointment.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(responseAppointment);
        responseText = await responseAppointment.Content.ReadAsStringAsync();
        var appointmentResult = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, responseAppointment.StatusCode);
        Assert.IsNotNull(appointmentResult);
        Assert.IsInstanceOfType(appointmentResult, typeof(Appointment));
        Assert.AreEqual(2, appointmentResult.SuggestedDates.Count);
        Assert.AreEqual(1, appointmentResult.Participants.Count);

        // Act
        var result = await client.DeleteAsync($"participant/{_customerId}/{appointmentId}/{Guid.NewGuid()}");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode);
    }

    [TestMethod]
    public async Task DeleteParticipants_AppointmentIsPasswordProtectedNoPasswordSubmitted_Unauthorized()
    {
        var password = "P@$$w0rd";

        HttpClient client = _testServer.CreateClient();
        var dto = await CreateTestAppointmentInDatabase(client, _customerId, password);

        //--- get the appointment
        var appointmentId = dto.AppointmentId;

        //--- add participiant
        Voting voting1 = new()
        {
            VotingId = Guid.Empty,
            CustomerId = _customerId,
            AppointmentId = dto.AppointmentId,
            ParticipantId = Guid.Empty,
            SuggestedDateId = dto.SuggestedDates.First().SuggestedDateId,
            Status = VotingStatusType.Accepted
        };

        var participants = dto.Participants ?? new List<Participant>();
        participants.Add(new()
        {
            AppointmentId = dto.AppointmentId,
            CustomerId = _customerId,
            ParticipantId = Guid.Empty,
            Name = "Joe",
            Votings = new List<Voting>() { voting1 }
        });

        // Act
        var credential = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{appointmentId}:{password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credential);

        var contentParticipant = new StringContent(JsonConvert.SerializeObject(participants), Encoding.UTF8,
            HttpConstants.TerminfinderMediaTypeJsonV1);
        var response = await client.PutAsync($"votings/{_customerId}/{appointmentId}", contentParticipant);
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        var responseText = await response.Content.ReadAsStringAsync();
        var responseparticipant = JsonConvert.DeserializeObject<Participant[]>(responseText);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(responseparticipant);

        // Act
        credential = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{appointmentId}:23rfgt4-We"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credential);

        var result = await client.DeleteAsync($"participant/{_customerId}/{appointmentId}/{responseparticipant[0].ParticipantId}");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode);
    }
}

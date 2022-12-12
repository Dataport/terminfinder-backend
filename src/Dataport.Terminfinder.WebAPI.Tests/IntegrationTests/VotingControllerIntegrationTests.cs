using Dataport.Terminfinder.WebAPI.Constants;
using System.Linq;
using System.Text;

namespace Dataport.Terminfinder.WebAPI.Tests.IntegrationTests;

[TestClass]
[TestCategory("Integrationtest")]
[ExcludeFromCodeCoverage]
public class VotingControllerIntegrationTests : BaseIntegrationTests
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
    public async Task AddAndUpdateParticipant_And_Votings()
    {
        var appointment = CreateTestAppointment(_customerId, Guid.Empty);
        SuggestedDate suggestedDate3 = new()
        {
            AppointmentId = Guid.Empty,
            CustomerId = _customerId,
            SuggestedDateId = Guid.Empty,
            StartDate = DateTime.Now.AddDays(3),
            EndDate = DateTime.Now.AddDays(4)
        };
        appointment.SuggestedDates.Add(suggestedDate3);

        HttpClient client = _testServer.CreateClient();

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8,
            HttpConstants.TerminfinderMediaTypeJsonV1);

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
        var appointmentId = dto.AppointmentId;

        // Act
        response = await client.GetAsync($"appointment/{_customerId}/{appointmentId}");
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        responseText = await response.Content.ReadAsStringAsync();
        dto = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(dto);

        var suggestedDates = dto.SuggestedDates.OrderBy(s => s.StartDate).ToList();

        Assert.IsInstanceOfType(dto, typeof(Appointment));
        Assert.AreEqual(appointmentId, dto.AppointmentId);
        Assert.AreEqual(3, dto.SuggestedDates.Count);

        //--- add participiant
        Voting voting1 = new()
        {
            VotingId = Guid.Empty,
            CustomerId = _customerId,
            AppointmentId = appointmentId,
            ParticipantId = Guid.Empty,
            SuggestedDateId = suggestedDates[0].SuggestedDateId,
            Status = VotingStatusType.Accepted
        };
        Voting voting2 = new()
        {
            VotingId = Guid.Empty,
            CustomerId = _customerId,
            AppointmentId = appointmentId,
            ParticipantId = Guid.Empty,
            SuggestedDateId = suggestedDates[1].SuggestedDateId,
            Status = VotingStatusType.Declined
        };
        Voting voting3 = new()
        {
            VotingId = Guid.Empty,
            CustomerId = _customerId,
            AppointmentId = appointmentId,
            ParticipantId = Guid.Empty,
            SuggestedDateId = suggestedDates[2].SuggestedDateId,
            Status = VotingStatusType.Questionable
        };

        var participants = appointment.Participants ?? new List<Participant>();
        participants.Add(new()
        {
            AppointmentId = appointmentId,
            CustomerId = _customerId,
            ParticipantId = Guid.Empty,
            Name = "Joe",
            Votings = new List<Voting>() { voting1, voting2, voting3 }
        });

        // Act
        var contentParticipant = new StringContent(JsonConvert.SerializeObject(participants), Encoding.UTF8,
            HttpConstants.TerminfinderMediaTypeJsonV1);
        response = await client.PutAsync($"votings/{_customerId}/{appointmentId}", contentParticipant);
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        responseText = await response.Content.ReadAsStringAsync();
        var responseparticipant = JsonConvert.DeserializeObject<Participant[]>(responseText);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(responseparticipant);
        Assert.IsInstanceOfType(responseparticipant, typeof(Participant[]));
        Assert.AreEqual(1, responseparticipant.Length);
        Assert.AreEqual(3, responseparticipant[0].Votings.Count);

        //--- get appointment
        response = await client.GetAsync($"appointment/{_customerId}/{appointmentId}");
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        responseText = await response.Content.ReadAsStringAsync();
        dto = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(dto);
        Assert.AreEqual(3, dto.Participants.First().Votings.Count);

        var votingChange = dto.Participants.First().Votings
            .First(v => v.SuggestedDateId == suggestedDates[2].SuggestedDateId);
        Assert.AreEqual(VotingStatusType.Questionable, votingChange.Status);

        votingChange.Status = VotingStatusType.Accepted;

        // Act
        contentParticipant = new StringContent(JsonConvert.SerializeObject(dto.Participants), Encoding.UTF8,
            HttpConstants.TerminfinderMediaTypeJsonV1);
        response = await client.PutAsync($"votings/{_customerId}/{appointmentId}", contentParticipant);
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        responseText = await response.Content.ReadAsStringAsync();
        responseparticipant = JsonConvert.DeserializeObject<Participant[]>(responseText);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(responseparticipant);
        Assert.IsInstanceOfType(responseparticipant, typeof(Participant[]));
        Assert.AreEqual(1, responseparticipant.Length);
        Assert.AreEqual(3, responseparticipant[0].Votings.Count);
        votingChange = responseparticipant[0].Votings
            .First(v => v.SuggestedDateId == suggestedDates[2].SuggestedDateId);
        Assert.AreEqual(VotingStatusType.Accepted, votingChange.Status);

        // Act Get voting
        response = await client.GetAsync($"votings/{_customerId}/{appointmentId}");
        response.EnsureSuccessStatusCode();
        var participantsWithVotings = JsonConvert.DeserializeObject<Participant[]>(responseText);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.IsNotNull(participantsWithVotings);
        Assert.IsInstanceOfType(participantsWithVotings, typeof(Participant[]));
        Assert.AreEqual(1, participantsWithVotings.Length);
        Assert.AreEqual(3, participantsWithVotings[0].Votings.Count);
    }

    [TestMethod]
    public async Task AppointmentIsPasswordProtectedAndNoPasswordSubmitted_Unauthorized()
    {
        var password = "P@$$w0rd";

        var appointment = CreateTestAppointment(_customerId, Guid.Empty, password);
        HttpClient client = _testServer.CreateClient();

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8,
            HttpConstants.TerminfinderMediaTypeJsonV1);

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

        // Act Get voting
        response = await client.GetAsync($"votings/{_customerId}/{dto.AppointmentId}");

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task Put_AppointmentIsPasswordProtectedAndNoPasswordSubmitted_Unauthorized()
    {
        var password = "P@$$w0rd";

        var appointment = CreateTestAppointment(_customerId, Guid.Empty, password);
        HttpClient client = _testServer.CreateClient();

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8,
            HttpConstants.TerminfinderMediaTypeJsonV1);

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

        var participants = appointment.Participants ?? new List<Participant>();
        participants.Add(new()
        {
            AppointmentId = dto.AppointmentId,
            CustomerId = _customerId,
            ParticipantId = Guid.Empty,
            Name = "Joe",
            Votings = new List<Voting>()
        });

        // Act Put voting
        var contentParticipant = new StringContent(JsonConvert.SerializeObject(participants), Encoding.UTF8,
            HttpConstants.TerminfinderMediaTypeJsonV1);
        response = await client.PutAsync($"votings/{_customerId}/{dto.AppointmentId}", contentParticipant);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task Put_AppointmentStatusIsPaused_NotFound()
    {
        var appointment = CreateTestAppointment(_customerId, Guid.Empty);
        appointment.AppointmentStatus = AppointmentStatusType.Paused;
        HttpClient client = _testServer.CreateClient();

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8,
            HttpConstants.TerminfinderMediaTypeJsonV1);

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

        var participants = appointment.Participants ?? new List<Participant>();
        participants.Add(new()
        {
            AppointmentId = dto.AppointmentId,
            CustomerId = _customerId,
            ParticipantId = Guid.Empty,
            Name = "Joe",
            Votings = new List<Voting>()
        });

        // Act Put voting
        var contentParticipant = new StringContent(JsonConvert.SerializeObject(participants), Encoding.UTF8,
            HttpConstants.TerminfinderMediaTypeJsonV1);
        response = await client.PutAsync($"votings/{_customerId}/{dto.AppointmentId}", contentParticipant);

        // Assert
        Assert.IsNotNull(response);
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
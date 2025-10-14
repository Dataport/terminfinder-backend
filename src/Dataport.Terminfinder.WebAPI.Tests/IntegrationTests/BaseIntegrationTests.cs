using Dataport.Terminfinder.WebAPI.Constants;
using System.Text;

namespace Dataport.Terminfinder.WebAPI.Tests.IntegrationTests;

[ExcludeFromCodeCoverage]
public abstract class BaseIntegrationTests
{
    /// <summary>
    /// ConfigurationBuilder for integrationstest
    /// </summary>
    /// <returns></returns>
    protected static IConfiguration GetConfigurationBuilder()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true)
            .Build();
        return config;
    }

    protected static Appointment CreateTestAppointment(Guid customerId, Guid adminId, string password = null)
    {
        var appointment = new Appointment
        {
            AppointmentId = Guid.Empty,
            CreatorName = "Tom",
            CustomerId = customerId,
            AdminId = adminId,
            Subject = "new",
            Description = "whats new",
            Place = "Hamburg",
            AppointmentStatus = AppointmentStatusType.Started,
            Password = password
        };

        var currentDateStartDate = DateTime.Now;
        var currentDateEndDate = DateTime.Now.AddDays(2);

        var suggestedDate1 = new SuggestedDate
        {
            AppointmentId = Guid.Empty,
            CustomerId = customerId,
            SuggestedDateId = Guid.Empty,
            StartDate = currentDateStartDate,
            EndDate = currentDateStartDate.AddDays(1)
        };

        var suggestedDate2 = new SuggestedDate
        {
            AppointmentId = Guid.Empty,
            CustomerId = customerId,
            SuggestedDateId = Guid.Empty,
            StartDate = currentDateStartDate.AddDays(2),
            StartTime = new DateTimeOffset(currentDateStartDate.Year, currentDateStartDate.Month, currentDateStartDate.Day, 20, 05, 0, new TimeSpan(1, 0, 0)),
            EndDate = new DateTime(currentDateEndDate.Year, currentDateEndDate.Month, currentDateEndDate.Day, 0, 0, 0, DateTimeKind.Local),
            EndTime = new DateTimeOffset(currentDateEndDate.Year, currentDateEndDate.Month, currentDateEndDate.Day, 21, 05, 0, new TimeSpan(1, 0, 0))
        };
        appointment.SuggestedDates = new List<SuggestedDate>
        {
            suggestedDate1,
            suggestedDate2
        };

        return appointment;
    }

    protected static async Task<Appointment> CreateTestAppointmentInDatabase(
        HttpClient client,
        Guid customerId,
        string password = null,
        Appointment appointment = null
    )
    {
        appointment ??= CreateTestAppointment(customerId, Guid.Empty, password);

        // Act
        var content = new StringContent(JsonConvert.SerializeObject(appointment), Encoding.UTF8, HttpConstants.TerminfinderMediaTypeJsonV1);

        var response = await client.PostAsync($"appointment/{customerId}", content);
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.IsNotNull(response);
        var responseText = await response.Content.ReadAsStringAsync();
        var dto = JsonConvert.DeserializeObject<Appointment>(responseText);
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        Assert.IsNotNull(dto);
        Assert.IsInstanceOfType(dto, typeof(Appointment));
        Assert.IsNotNull(dto.AppointmentId);

        return dto;
    }
}
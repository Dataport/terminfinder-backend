using Dataport.Terminfinder.BusinessObject.JsonSerializer;

namespace Dataport.Terminfinder.BusinessObject;

/// <summary>
/// Representation for an appointment protection result
/// </summary>
[ExcludeFromCodeCoverage]
public class AppointmentProtectionResult
{
    /// <summary>
    /// Identifier of an appointment
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000000</example>
    [JsonConverter(typeof(GuidNullConverter))]
    [JsonProperty(PropertyName = "appointmentId")]
    public Guid AppointmentId { get; set; }

    /// <summary>
    /// Information if the appointment is protected by a password.
    /// True the appointment is protected by a password. False the appointment is not protected by a password.
    /// If the appointment is protected by a password, the password has to be submitted in the basic authentication header of the http request
    /// if the appointment should be read or altered.
    /// </summary>
    /// <example>true</example>
    [JsonProperty(PropertyName = "protected")]
    public bool IsProtectedByPassword { get; set; }
}
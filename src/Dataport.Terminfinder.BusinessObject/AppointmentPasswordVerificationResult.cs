using Dataport.Terminfinder.BusinessObject.JsonSerializer;

namespace Dataport.Terminfinder.BusinessObject;

/// <summary>
/// Representation for an appointment password verification result
/// </summary>
[ExcludeFromCodeCoverage]
public class AppointmentPasswordVerificationResult
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

    /// <summary>
    /// Information if the password of the protected appointment is valid.
    /// True the appointment is protected by a password and the password is identical. False the appointment is not protected by a password or the password is wrong.
    /// If the appointment is protected by a password, the password has to be submitted in the basic authentication header of the http request
    /// if the appointment should be read or altered.
    /// </summary>
    /// <example>true</example>
    [JsonProperty(PropertyName = "passwordvalidation")]
    public bool IsPasswordValid { get; set; }
}
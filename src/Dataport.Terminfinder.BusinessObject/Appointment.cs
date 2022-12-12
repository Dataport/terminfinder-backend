using System.Collections.Generic;
using Dataport.Terminfinder.BusinessObject.Enum;
using Dataport.Terminfinder.BusinessObject.JsonSerializer;
using Dataport.Terminfinder.BusinessObject.Validators;
using Dataport.Terminfinder.Common.Extension;

namespace Dataport.Terminfinder.BusinessObject;

/// <summary>
/// Representation for an appointment
/// </summary>
[ExcludeFromCodeCoverage]
[Table("appointment", Schema = "public")]
public class Appointment
{
    /// <summary>
    /// Identifier of an appointment
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000000</example>
    [Column("appointmentid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonConverter(typeof(GuidNullConverter))]
    [JsonProperty(PropertyName = "appointmentId")]
    public Guid AppointmentId { get; set; }

    /// <summary>
    /// Identifier of the customer
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000000</example>
    [Column("customerid")]
    [JsonIgnore]
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Customer
    /// </summary>
    [JsonIgnore]
    public Customer Customer { get; set; }

    /// <summary>
    /// Identifier to administrate the appointment
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000000</example>
    [Column("adminid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonConverter(typeof(GuidNullConverter))]
    [JsonProperty(PropertyName = "adminId")]
    public Guid AdminId { get; set; }

    /// <summary>
    /// Name of the creator of the appointment
    /// </summary>
    /// <example>Jane Doe</example>
    [Column("creatorname")]
    [MaxLength(100, ErrorMessage = "The creator name must be a string with a maximum length of 100 characters.")]
    [Required(ErrorMessage = "The creator name is required.")]
    [RegularExpression(
        @"^[0-9a-zA-ZáàâäãçéèêëíìîïñóòôöõúùûüýÿßœÁÀÂÄÃÇÉÈÊËÍÌÎÏÑÓÒÔÖÕÚÙÛÜÝŸŒøæØåÆÅ´`\'+\-_,.;:!?§$€@#\/()=%&*\"" ]*$",
        ErrorMessage = "The creator name contains invalid signs.")]
    [JsonProperty(PropertyName = "creatorName")]
    public string CreatorName { get; set; }

    /// <summary>
    /// creation date of the row set
    /// </summary>
    [Column("creationdate")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public DateTime CreationDate { get; set; }
    /// <summary>
    /// Subject of the appointment
    /// </summary>
    /// <example>My first appointment</example>
    [Column("subject")]
    [MaxLength(300, ErrorMessage = "The subject must be a string with a maximum length of 300 characters.")]
    [Required(ErrorMessage = "The subject is required.")]
    [JsonProperty(PropertyName = "subject")]
    public string Subject { get; set; }

    /// <summary>
    /// Description of the appointment
    /// </summary>
    /// <example>A description for the appointment</example>
    [Column("description")]
    [MaxLength(1500, ErrorMessage = "The description must be a string with a maximum length of 1500 characters.")]
    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }

    /// <summary>
    /// Place of the appointment, following characters are valid: all digits, all letters and signs áàâäãçéèêëíìîïñóòôöõúùûüýÿßœÁÀÂÄÃÇÉÈÊËÍÌÎÏÑÓÒÔÖÕÚÙÛÜÝŸŒøæØåÆÅ´`'+-_,.;:!?§$€@#/()=% &amp;*"
    /// </summary>
    /// <remarks>Hello</remarks>
    /// <example>At Jane Doe's home</example>
    [Column("place")]
    [MaxLength(300, ErrorMessage = "The place must be a string with a maximum length of 300 characters.")]
    [RegularExpression(
        @"^[0-9a-zA-ZáàâäãçéèêëíìîïñóòôöõúùûüýÿßœÁÀÂÄÃÇÉÈÊËÍÌÎÏÑÓÒÔÖÕÚÙÛÜÝŸŒøæØåÆÅ´`\'+\-_,.;:!?§$€@#\/()=%&*\"" ]*$",
        ErrorMessage = "The place contains invalid signs.")]
    [JsonProperty(PropertyName = "place")]
    public string Place { get; set; }

    /// <summary>
    /// Status of the appointment
    /// </summary>
    [Column("status")]
    [Required(ErrorMessage = "The status is required.")]
    [JsonIgnore]
    [MaxLength(20)]
    public string StatusIdentifier
    {
        get => AppointmentStatus.ToString();
        set => AppointmentStatus = value.ToEnum<AppointmentStatusType>();
    }

    /// <summary>
    /// Status of an appointment
    /// </summary>
    /// <example>started</example>
    [NotMapped]
    [JsonProperty(PropertyName = "status")]
    [JsonConverter(typeof(StringEnumLowerCamelCaseConverter))]
    public AppointmentStatusType AppointmentStatus { get; set; }

    /// <summary>
    /// Optional password of an appointment.
    /// the hash value of the password are stored in the database
    /// A password must be string with a length from 8 to 30 characters and must contain an uppercase letter, a digit and a special character.
    /// The password won't be returned from the api.
    /// </summary>
    /// <example>Pa$$w0rd</example>
    [Column("password")]
    [Password(ErrorMessage =
        "The password must be a string with a length from 8 to 30 characters and must contain an uppercase letter, a digit and a special character.")]
    [JsonProperty(PropertyName = "password")]
    [MaxLength(120)]
    public string Password { get; set; }

    /// <summary>
    /// All suggested dates
    /// </summary>
    [EnsureMaximumElements(MaxElements = 100, ErrorMessage = "Up to 100 suggested dates are allowed.")]
    public ICollection<SuggestedDate> SuggestedDates { get; set; }

    /// <summary>
    /// List of the participants
    /// </summary>
    [EnsureMaximumElements(MaxElements = 5000, ErrorMessage = "Up to 5000 participants are allowed.")]
    public ICollection<Participant> Participants { get; set; }
}
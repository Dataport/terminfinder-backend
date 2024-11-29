using Dataport.Terminfinder.BusinessObject.JsonSerializer;

namespace Dataport.Terminfinder.BusinessObject;

/// <summary>
/// One participant for one appointment
/// </summary>
[ExcludeFromCodeCoverage]
[Table("participant", Schema = "public")]
public class Participant
{
    /// <summary>
    /// Identifier of the participant
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000000</example>
    [Column("participantid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonConverter(typeof(GuidNullConverter))]
    [JsonProperty(PropertyName = "participantId")]
    public Guid ParticipantId { get; set; }

    /// <summary>
    /// Identifier of the assigned appointment
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000000</example>
    [Column("appointmentid")]
    [JsonIgnore]
    public Guid AppointmentId { get; set; }

    /// <summary>
    /// Appointment
    /// </summary>
    [JsonIgnore]
    public Appointment Appointment { get; set; }

    /// <summary>
    /// Identifier of the assigned appointment
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
    /// creation date of the row set
    /// </summary>
    [Column("creationdate")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Name of a participant
    /// </summary>
    [Column("name")]
    [MaxLength(100, ErrorMessage = "The name must be a string with a maximum length of 100 characters.")]
    [Required(ErrorMessage = "The name is required.")]
    [RegularExpression(
        @"^[0-9a-zA-ZáàâäãçéèêëíìîïñóòôöõúùûüýÿßœÁÀÂÄÃÇÉÈÊËÍÌÎÏÑÓÒÔÖÕÚÙÛÜÝŸŒøæØåÆÅ´`\'+\-_,.;:!?§$€@#\/()=%&*\"" ]*$",
        ErrorMessage = "The name contains invalid signs.")]
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// List of all votings
    /// </summary>
    public ICollection<Voting> Votings { get; set; }

    /// <summary>
    /// Check if votings are valid
    /// </summary>
    [JsonIgnore]
    public bool IsValid
    {
        get
        {
            if (Votings == null)
            {
                return true;
            }

            bool result = true;

            foreach (Voting vote in Votings)
            {
                result = result && vote.IsValid;
            }

            return result;
        }
    }
}
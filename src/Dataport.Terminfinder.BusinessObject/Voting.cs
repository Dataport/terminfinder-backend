using Dataport.Terminfinder.BusinessObject.Enum;
using Dataport.Terminfinder.BusinessObject.JsonSerializer;
using Dataport.Terminfinder.Common.Extension;

namespace Dataport.Terminfinder.BusinessObject;

/// <summary>
/// Infos of one voting for one suggested date, n:m between participant and Suggested date
/// </summary>
[ExcludeFromCodeCoverage]
[Table("voting", Schema = "public")]
public class Voting
{
    /// <summary>
    /// Identifier of the voting
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000000</example>
    [Column("votingid")]
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonConverter(typeof(GuidNullConverter))]
    [JsonProperty(PropertyName = "votingId")]
    public Guid VotingId { get; set; }

    /// <summary>
    /// Identifier of the assigned participant
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000000</example>
    [Column("participantid")]
    [JsonConverter(typeof(GuidNullConverter))]
    [JsonProperty(PropertyName = "participantId")]
    public Guid ParticipantId { get; set; }

    /// <summary>
    /// Participant
    /// </summary>
    [JsonIgnore]
    public Participant Participant { get; set; }

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
    /// Identifier of the assigned customer
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
    /// Identifier of the assigned suggested date
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000000</example>
    [Column("suggesteddateid")]
    [JsonConverter(typeof(GuidNullConverter))]
    [JsonProperty(PropertyName = "suggestedDateId")]
    public Guid SuggestedDateId { get; set; }

    /// <summary>
    /// SuggestedDate
    /// </summary>
    [JsonIgnore]
    public SuggestedDate SuggestedDate { get; set; }

    /// <summary>
    /// creation date of the row set
    /// </summary>
    [Column("creationdate")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Status for the suggested date
    /// </summary>
    [Column("status")]
    [Required(ErrorMessage = "The status is required")]
    [JsonIgnore]
    [MaxLength(20)]
    public string StatusIdentifier
    {
        get => Status.ToString();
        set => Status = value.ToEnum<VotingStatusType>();
    }

    /// <summary>
    /// Status for the suggested date
    /// </summary>
    /// <example>accepted</example>
    [NotMapped]
    [JsonProperty(PropertyName = "status")]
    [JsonConverter(typeof(StringEnumLowerCamelCaseConverter))]
    public VotingStatusType Status { get; set; }

    /// <summary>
    /// Check if status of voting are valid
    /// </summary>
    [JsonIgnore]
    public bool IsValid => System.Enum.TryParse(StatusIdentifier, true, out VotingStatusType _);
}
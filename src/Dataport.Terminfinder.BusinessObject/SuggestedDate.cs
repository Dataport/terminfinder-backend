using Dataport.Terminfinder.BusinessObject.JsonSerializer;
using Dataport.Terminfinder.BusinessObject.Validators;
using JetBrains.Annotations;

namespace Dataport.Terminfinder.BusinessObject;

/// <summary>
/// Infos of one suggested date
/// </summary>
[ExcludeFromCodeCoverage]
[Table("suggesteddate", Schema = "public")]
public class SuggestedDate
{
    /// <summary>
    /// Identifier of one suggested date
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000000</example>
    [Column("suggesteddateid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonConverter(typeof(GuidNullConverter))]
    [JsonProperty(PropertyName = "suggestedDateId")]
    public Guid SuggestedDateId { get; set; }

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
    /// creation date of the row set
    /// </summary>
    [Column("creationdate", TypeName = "timestamp with time zone")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// Start date of the suggested date
    /// </summary>
    /// <example>2018-11-10</example>
    [Column("startdate", TypeName = "date")]
    [DateTodayOrFuture(ErrorMessage = "The start date is in the past.")]
    [JsonProperty(PropertyName = "startDate")]
    [JsonConverter(typeof(DateConverter))]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Optional start time of the suggested date
    /// </summary>
    /// <example>02:24:12.958+01:00</example>
    [Column("starttime", TypeName = "time with time zone")]
    [JsonProperty(PropertyName = "startTime")]
    [JsonConverter(typeof(TimeConverter))]
    public DateTimeOffset? StartTime { get; set; }

    /// <summary>
    /// Optional end date of the suggested date
    /// </summary>
    /// <example>2018-11-10</example>
    [Column("enddate", TypeName = "date")]
    [DateTodayOrFuture(ErrorMessage = "The end date is in the past.")]
    [JsonProperty(PropertyName = "endDate")]
    [JsonConverter(typeof(DateConverter))]
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Optional end time of the suggested date
    /// </summary>
    /// <example>02:24:12.958+01:00</example>
    [Column("endtime", TypeName = "time with time zone")]
    [JsonProperty(PropertyName = "endTime")]
    [JsonConverter(typeof(TimeConverter))]
    public DateTimeOffset? EndTime { get; set; }

    /// <summary>
    /// Optional description of the suggested date
    /// </summary>
    [Column("description")]
    [MaxLength(100, ErrorMessage = "The description must be a string with a maximum length of 100 characters.")]
    [JsonProperty(PropertyName = "description")]
    [CanBeNull]
    public string Description { get; set; }

    /// <summary>
    /// List of all votings
    /// </summary>
    [JsonIgnore]
    public ICollection<Voting> Votings { get; set; }

    /// <summary>
    /// Indicates if participants have voted on this suggested date
    /// </summary>
    [NotMapped]
    [JsonProperty(PropertyName = "hasVotings")]
    public bool HasVotings => Votings.Count != 0;
}
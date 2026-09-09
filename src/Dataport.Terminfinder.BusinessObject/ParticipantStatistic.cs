namespace Dataport.Terminfinder.BusinessObject;

/// <summary>
/// Representation for a Participant statistic
/// </summary>
[ExcludeFromCodeCoverage]
[Table("participantStatistic", Schema = "public")]
public class ParticipantStatistic
{
    /// <summary>
    /// Identifier of the participant statistic
    /// </summary>
    [Column("participantStatisticId")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid ParticipantStatisticId { get; set; }

    /// <summary>
    /// Identifier of the customer
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000000</example>
    [Column("customerId")]
    public Guid CustomerId { get; set; }

    /// <summary>
    /// The year and month for which the statistic is recorded
    /// </summary>
    [Column("yearMonth")]
    public DateOnly YearMonth { get; set; }

    /// <summary>
    /// The count for the recorded statistic
    /// </summary>
    [Column("count")]
    public int Count { get; set; } = 1;
}
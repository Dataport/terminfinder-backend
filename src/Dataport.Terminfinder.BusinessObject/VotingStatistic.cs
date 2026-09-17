namespace Dataport.Terminfinder.BusinessObject;

/// <summary>
/// Representation for a Voting statistic
/// </summary>
[ExcludeFromCodeCoverage]
[Table("votingstatistic", Schema = "public")]
public class VotingStatistic
{
    /// <summary>
    /// Identifier of the voting statistic
    /// </summary>
    [Column("votingstatisticid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid VotingStatisticId { get; set; }

    /// <summary>
    /// Identifier of the customer
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000000</example>
    [Column("customerid")]
    public Guid CustomerId { get; set; }

    /// <summary>
    /// The year and month for which the statistic is recorded
    /// </summary>
    [Column("yearmonth")]
    public DateOnly YearMonth { get; set; }

    /// <summary>
    /// The count for the recorded statistic
    /// </summary>
    [Column("count")]
    public int Count { get; set; } = 1;
}
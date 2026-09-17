namespace Dataport.Terminfinder.BusinessObject;

/// <summary>
/// Representation for an appointment statistic
/// </summary>
[ExcludeFromCodeCoverage]
[Table("appointmentstatistic", Schema = "public")]
public class AppointmentStatistic
{
    /// <summary>
    /// Identifier of the appointment statistic
    /// </summary>
    [Column("appointmentstatisticid")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid AppointmentStatisticId { get; set; }

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
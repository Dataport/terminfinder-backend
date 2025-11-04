using Dataport.Terminfinder.BusinessObject.JsonSerializer;

namespace Dataport.Terminfinder.BusinessObject;

/// <summary>
/// Entity Class for Legacy Customer
/// </summary>
[ExcludeFromCodeCoverage]
[Table("legacycustomer", Schema = "public")]
public class LegacyCustomer
{
    /// <summary>
    /// Identifier of the customer
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000000</example>
    [Column("customerid")]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonConverter(typeof(GuidNullConverter))]
    [JsonProperty(PropertyName = "customerId")]
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Host address where the legacy customer can be reached
    /// </summary>
    /// <example>Any customer</example>
    [Column("hostaddress")]
    [MaxLength(100)]
    [Required]
    [JsonProperty(PropertyName = "hostaddress")]
    public string HostAddress { get; set; }

    /// <summary>
    /// Status of the legacy customer
    /// </summary>
    [Column("status")]
    [Required]
    [JsonProperty(PropertyName = "status")]
    [MaxLength(20)]
    public string Status { get; set; }

    /// <summary>
    /// creation date of the row set
    /// </summary>
    [Column("creationdate")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public DateTime CreationDate { get; set; }
}

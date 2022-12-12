using Dataport.Terminfinder.BusinessObject.JsonSerializer;

namespace Dataport.Terminfinder.BusinessObject;

/// <summary>
/// Entity Class for Customer
/// </summary>
[ExcludeFromCodeCoverage]
[Table("customer", Schema = "public")]
public class Customer
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
    /// Name of the customer
    /// </summary>
    /// <example>Any customer</example>
    [Column("customername")]
    [MaxLength(100)]
    [Required]
    [JsonProperty(PropertyName = "customerName")]
    public string CustomerName { get; set; }

    /// <summary>
    /// Status of the customer
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

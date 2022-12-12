namespace Dataport.Terminfinder.BusinessObject;

/// <summary>
/// Configurationvalues
/// </summary>
[ExcludeFromCodeCoverage]
[Table("appconfig", Schema = "public")]
public class AppConfig
{
    /// <summary>
    /// Key
    /// </summary>
    [Column("configkey")]
    [Key]
    [Required]
    [MaxLength(100)]
    public string Key { get; set; }

    /// <summary>
    /// Value
    /// </summary>
    [Column("configvalue")]
    [MaxLength(300)]
    public string Value { get; set; }
}
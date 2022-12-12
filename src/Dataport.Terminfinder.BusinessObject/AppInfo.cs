namespace Dataport.Terminfinder.BusinessObject;

/// <summary>
/// Application Infos
/// </summary>
[ExcludeFromCodeCoverage]
public class AppInfo
{
    /// <summary>
    /// Version number of the api
    /// </summary>
    /// <example>1.0.1</example>
    [JsonProperty(PropertyName = "version")]
    public string VersionNumber { get; set; }

    /// <summary>
    /// Build date of the version
    /// </summary>
    /// <example>2017-01-01</example>
    [JsonProperty(PropertyName = "builddate")]
    public string BuildDate { get; set; }
}
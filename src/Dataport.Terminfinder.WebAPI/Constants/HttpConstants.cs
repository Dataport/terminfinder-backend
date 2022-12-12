namespace Dataport.Terminfinder.WebAPI.Constants;

/// <summary>
/// Class for http constants
/// </summary>
public static class HttpConstants
{
    private const string TerminfinderMediaTypeV1Prefix = "application/terminfinder.api-v1";

    private const string Utf8CharsetSuffix = "charset=utf-8";

    /// <summary>
    /// Custom media type (json format) for version 1 for Terminfinder
    /// </summary>
    public const string TerminfinderMediaTypeJsonV1 = TerminfinderMediaTypeV1Prefix + "+json";

    /// <summary>
    /// Custom media type (json format) for version 1 for Terminfinder
    /// </summary>
    public const string TerminfinderMediaTypeJsonV1WithUtf8CharsetSuffix =
        TerminfinderMediaTypeJsonV1 + "; " + Utf8CharsetSuffix;
}
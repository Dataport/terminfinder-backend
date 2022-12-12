namespace Dataport.Terminfinder.BusinessObject.Error;

///  <inheritdoc />
[ExcludeFromCodeCoverage]
public class ErrorResult : IErrorResult
{
    /// <inheritdoc />
    [JsonProperty(PropertyName = "code")]
    public string Code { get; set; }

    ///  <inheritdoc />
    [JsonProperty(PropertyName = "message")]
    public string Message { get; set; }

    ///  <inheritdoc />
    [JsonProperty(PropertyName = "language")]
    public string Language { get; set; }
}
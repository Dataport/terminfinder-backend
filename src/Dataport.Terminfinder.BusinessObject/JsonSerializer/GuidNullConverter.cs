namespace Dataport.Terminfinder.BusinessObject.JsonSerializer;

/// <inheritdoc />
public class GuidNullConverter : JsonConverter
{
    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
    {
        string result = null;

#pragma warning disable CA1806 // Do not ignore method results
        Guid.TryParse(value?.ToString(), out var guid);
#pragma warning restore CA1806 // Do not ignore method results

        if (guid != Guid.Empty)
        {
            result = guid.ToString();
        }

        writer.WriteValue(result);
    }

    /// <inheritdoc />
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        Newtonsoft.Json.JsonSerializer serializer)
    {
        var value = reader?.Value;

#pragma warning disable CA1806 // Do not ignore method results
        Guid.TryParse(value?.ToString(), out var guid);
#pragma warning restore CA1806 // Do not ignore method results
        return guid;
    }

    /// <inheritdoc />
    public override bool CanRead => true;

    /// <inheritdoc />
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Guid);
    }
}
using Dataport.Terminfinder.Common.Extension;

namespace Dataport.Terminfinder.BusinessObject.JsonSerializer;

/// <inheritdoc />
public class StringEnumLowerCamelCaseConverter : JsonConverter
{
    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
    {
        var result = value?.ToString()?.FirstCharacterToLower();
        writer.WriteValue(result);
    }

    /// <inheritdoc />
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        Newtonsoft.Json.JsonSerializer serializer)
    {
        var value = reader?.Value;
        return System.Enum.TryParse(objectType, value?.ToString(), true, out var result)
            ? result
            : Activator.CreateInstance(objectType);
    }

    /// <inheritdoc />
    public override bool CanRead => true;

    /// <inheritdoc />
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsEnum;
    }
}
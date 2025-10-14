using System.Globalization;

namespace Dataport.Terminfinder.BusinessObject.JsonSerializer;

/// <inheritdoc />
public class TimeConverter : JsonConverter
{
    /// <summary>
    /// Time format for converting
    /// </summary>
    public static readonly string TimeFormat = "HH:mm:ss.fffzzz";

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
    {
        string result = null;

        if (value is DateTimeOffset date)
        {
            result = date.ToUniversalTime().ToString(TimeFormat);
        }

        writer.WriteValue(result);
    }

    /// <inheritdoc />
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        Newtonsoft.Json.JsonSerializer serializer)
    {
        DateTimeOffset? result = null;

        switch (reader.Value)
        {
            case null:
                return null;
            case string value:
                result = DateTimeOffset.ParseExact(value, TimeFormat, CultureInfo.InvariantCulture).UtcDateTime;
                break;
        }

        return result;
    }

    /// <inheritdoc />
    public override bool CanRead => true;

    /// <inheritdoc />
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DateTimeOffset?) || objectType == typeof(DateTimeOffset);
    }
}
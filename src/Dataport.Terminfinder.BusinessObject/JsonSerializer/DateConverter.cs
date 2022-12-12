using System.Globalization;

namespace Dataport.Terminfinder.BusinessObject.JsonSerializer;

/// <inheritdoc />
public class DateConverter : JsonConverter
{
    /// <summary>
    /// Date format for converting
    /// </summary>
    public static readonly string DateFormat = "yyyy-MM-dd";

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
    {
        string result = null;

        if (value is DateTime date)
        {
            result = date.ToString(DateFormat);
        }

        writer.WriteValue(result);
    }

    /// <inheritdoc />
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        Newtonsoft.Json.JsonSerializer serializer)
    {
        DateTime? result = null;

        switch (reader?.Value)
        {
            case null:
                return null;
            case string value:
                result = DateTime.ParseExact(value, DateFormat, CultureInfo.InvariantCulture).Date;
                break;
        }

        return result;
    }

    /// <inheritdoc />
    public override bool CanRead => true;

    /// <inheritdoc />
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DateTime?) || objectType == typeof(DateTime);
    }
}
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RaceCalendar.Api.JsonConverters;

public class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    private readonly string _serializationFormat = "HH:mm:ss.fff";

    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return TimeOnly.Parse(value!);
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(_serializationFormat, CultureInfo.InvariantCulture));
    }
}

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RaceCalendar.Api.JsonConverters;

public class DateOnlyConverter : JsonConverter<DateOnly>
{
    private readonly string _serializationFormat = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return DateOnly.Parse(value!);
    }
    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(_serializationFormat, CultureInfo.InvariantCulture));
    }
}

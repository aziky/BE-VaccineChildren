using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class DateOnlyJsonConverter : JsonConverter<DateOnly?>
{
    private const string Format = "dd-MM-yyyy";

    public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String && DateOnly.TryParseExact(reader.GetString(), Format, null, System.Globalization.DateTimeStyles.None, out var date))
        {
            return date;
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString(Format));
        }
    }
}

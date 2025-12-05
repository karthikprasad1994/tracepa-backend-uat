using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public class FlexibleDateTimeConverter : JsonConverter<DateTime>
{
    private static readonly string[] SupportedFormats = new[]
    {
        "yyyy-MM-ddTHH:mm:ss.FFFFFFFK",
        "yyyy-MM-dd",
        "dd-MM-yyyy",
        "dd/MM/yyyy",
        "MM/dd/yyyy",
        "MM-dd-yyyy"
    };

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (string.IsNullOrWhiteSpace(value))
            return default;

        if (DateTime.TryParseExact(value, SupportedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return date;

        if (DateTime.TryParse(value, out date))
            return date;

        throw new JsonException($"Unable to parse DateTime from '{value}'");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("dd-MM-yyyy")); // << desired output
    }
}

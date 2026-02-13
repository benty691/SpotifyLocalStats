using System.Text.Json;
using System.Text.Json.Serialization;

public class UnixTimestampConverter : JsonConverter<DateTime?>
{
    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.Number)
        {
            var unixTimestamp = reader.GetInt64();
            return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;
        }

        throw new JsonException("Expected number for Unix timestamp");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            var unixTimestamp = new DateTimeOffset(value.Value).ToUnixTimeSeconds();
            writer.WriteNumberValue(unixTimestamp);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
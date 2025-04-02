using System.Text.Json.Serialization;
using System.Text.Json;
namespace Domain.Models
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String &&
                TimeSpan.TryParse(reader.GetString(), out var timeSpan))
            {
                return timeSpan;
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                return TimeSpan.FromSeconds(reader.GetDouble());
            }

            throw new JsonException("Invalid TimeSpan format.");
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }



}

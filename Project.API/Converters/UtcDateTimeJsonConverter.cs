using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Project.API.Converters
{
    public sealed class UtcDateTimeJsonConverter : JsonConverter<DateTime>
    {
        private const string OutputFormat = "yyyy-MM-ddTHH:mm:ssZ";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString()!, null, DateTimeStyles.RoundtripKind);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var utc = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime();
            writer.WriteStringValue(utc.ToString(OutputFormat, CultureInfo.InvariantCulture));
        }
    }
}

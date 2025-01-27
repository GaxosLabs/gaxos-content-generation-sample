using System;
using Newtonsoft.Json;

namespace ContentGeneration.Models
{
    internal class DateTimeFromUnixTimeStampConverter : JsonConverter<DateTime>
    {
        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        {
            writer.WriteValue((long)(value - DateTimeOffset.UnixEpoch).TotalSeconds);
        }

        public override DateTime ReadJson(
            JsonReader reader,
            Type objectType,
            DateTime existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = (long)reader.Value!;
            return DateTimeOffset.FromUnixTimeSeconds(value).UtcDateTime;
        }
    }
}
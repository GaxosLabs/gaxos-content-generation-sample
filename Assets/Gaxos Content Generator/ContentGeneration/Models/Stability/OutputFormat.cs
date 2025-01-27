using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Stability
{
    public enum OutputFormat
    {
        Png, Jpeg, Webp
    }

    internal class OutputFormatConverter : EnumJsonConverter<OutputFormat>
    {
        public override void WriteJson(JsonWriter writer, OutputFormat value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().ToLowerInvariant());
        }
    }
}
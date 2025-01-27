using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.ElevenLabs
{
    public enum TextNormalization
    {
        Auto, On, Off
    }
    internal class TextNormalizationConverter : EnumJsonConverter<TextNormalization>
    {
        public override void WriteJson(JsonWriter writer, TextNormalization value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().ToLowerInvariant());
        }
    }
}
using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.DallE
{
    public enum Style
    {
        Vivid, Natural
    }
    internal class StyleConverter : EnumJsonConverter<Style>
    {
        public override void WriteJson(JsonWriter writer, Style value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().ToLowerInvariant());
        }
    }
}
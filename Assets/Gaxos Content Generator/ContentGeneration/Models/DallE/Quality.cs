using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.DallE
{
    public enum Quality
    {
        Standard, Hd
    }
    internal class QualityConverter : EnumJsonConverter<Quality>
    {
        public override void WriteJson(JsonWriter writer, Quality value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().ToLowerInvariant());
        }
    }
}
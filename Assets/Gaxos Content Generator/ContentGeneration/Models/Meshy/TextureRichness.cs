using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Meshy
{
    public enum TextureRichness
    {
        High, Medium, Low, None
    }
    
    internal class TextureRichnessConverter : EnumJsonConverter<TextureRichness>
    {
        public override void WriteJson(JsonWriter writer, TextureRichness value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().ToLowerInvariant());
        }
    }
}
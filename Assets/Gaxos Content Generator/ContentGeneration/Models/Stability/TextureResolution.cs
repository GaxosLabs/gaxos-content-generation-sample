using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Stability
{
    public enum TextureResolution
    {
        _1024, _2048, _512
    }

    internal class TextureResolutionConverter : EnumJsonConverter<TextureResolution>
    {
        public override void WriteJson(JsonWriter writer, TextureResolution value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString()[1..]);
        }

        protected override string AdaptString(string str)
        {
            return "_" + str;
        }
    }
}
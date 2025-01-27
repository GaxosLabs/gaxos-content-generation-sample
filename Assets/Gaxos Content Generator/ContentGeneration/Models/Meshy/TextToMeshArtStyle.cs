using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Meshy
{
    public enum TextToMeshArtStyle
    {
        Realistic, Sculpture, Pbr
    }

    internal class TextToMeshArtStyleConverter : EnumJsonConverter<TextToMeshArtStyle>
    {
        public override void WriteJson(JsonWriter writer, TextToMeshArtStyle value, JsonSerializer serializer)
        {
            var str = value switch
            {
                _ => value.ToString().ToLowerInvariant(),
            };
            writer.WriteValue(str);
        }

        protected override string AdaptString(string str)
        {
            return base.AdaptString(str).Replace("-", "");
        }
    }
}
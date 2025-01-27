using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Stability
{
    public enum Mode
    {
        TextToImage,
        ImageToImage
    }

    internal class ModeConverter : EnumJsonConverter<Mode>
    {
        public override void WriteJson(JsonWriter writer, Mode value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().CamelCaseToDashes());
        }

        protected override string AdaptString(string str)
        {
            return str.DashesToCamelCase();
        }
    }
}
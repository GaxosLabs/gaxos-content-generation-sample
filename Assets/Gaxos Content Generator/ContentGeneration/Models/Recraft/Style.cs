using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Recraft
{
    public enum Style
    {
        RealisticImage, DigitalIllustration, VectorIllustration, Icon
    }
    internal class StyleConverter : EnumJsonConverter<Style>
    {
        public override void WriteJson(JsonWriter writer, Style value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().CamelCaseToUnderscores());
        }

        protected override string AdaptString(string str)
        {
            return base.AdaptString(str.Replace("_", ""));
        }
    }
}
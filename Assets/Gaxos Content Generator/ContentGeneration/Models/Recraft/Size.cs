using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Recraft
{
    public enum Size
    {
        _1024x1024,
        _1365x1024,
        _1024x1365,
        _1536x1024,
        _1024x1536,
        _1820x1024,
        _1024x1820,
        _1024x2048,
        _2048x1024,
        _1434x1024,
        _1024x1434,
        _1024x1280,
        _1280x1024,
        _1024x1707,
        _1707x1024,
    }

    internal class SizeConverter : EnumJsonConverter<Size>
    {
        public override void WriteJson(JsonWriter writer, Size value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString()[1..]);
        }

        protected override string AdaptString(string str)
        {
            return base.AdaptString("_" + str);
        }
    }
}
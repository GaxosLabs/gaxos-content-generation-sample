using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Meshy
{
    public enum Resolution
    {
        _1024, _2048, _4096 
    }

    internal class ResolutionConverter : EnumJsonConverter<Resolution>
    {
        public override void WriteJson(JsonWriter writer, Resolution value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().TrimStart('_'));
        }

        protected override string AdaptString(string str)
        {
            return "_" + base.AdaptString(str);
        }
    }
}
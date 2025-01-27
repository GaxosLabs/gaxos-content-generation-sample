using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.DallE
{
    public enum Model
    {
        DallE2, DallE3 
    }
    internal class ModelConverter : EnumJsonConverter<Model>
    {
        public override void WriteJson(JsonWriter writer, Model value, JsonSerializer serializer)
        {
            var str = value switch
            {
                Model.DallE2 => "dall-e-2",
                Model.DallE3 => "dall-e-3",
                _ => value.ToString().ToUpperInvariant(),
            };
            writer.WriteValue(str);
        }

        protected override string AdaptString(string str)
        {
            return base.AdaptString(str).Replace("-", "");
        }
    }
}
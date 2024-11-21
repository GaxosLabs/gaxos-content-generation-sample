using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Suno
{
    public enum Model
    {
        ChirpV35,
        ChirpV3,
        ChirpV2,
    }
    internal class ModelConverter : EnumJsonConverter<Model>
    {
        public override void WriteJson(JsonWriter writer, Model value, JsonSerializer serializer)
        {
            var str = value switch
            {
                Model.ChirpV35 => "chirp-v3.5",
                Model.ChirpV3 => "chirp-v3",
                Model.ChirpV2 => "chirp-v2",
                _ => value.ToString().ToUpperInvariant(),
            };
            writer.WriteValue(str);
        }

        protected override string AdaptString(string str)
        {
            return base.AdaptString(str).Replace("-", "").Replace(".", "");
        }
    }
}
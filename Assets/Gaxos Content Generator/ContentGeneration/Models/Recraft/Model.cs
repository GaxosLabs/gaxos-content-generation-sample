using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Recraft
{
    public enum Model
    {
        Recraftv3, Recraft20b
    }
    internal class ModelConverter : EnumJsonConverter<Model>
    {
        public override void WriteJson(JsonWriter writer, Model value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().ToLower());
        }
    }
}
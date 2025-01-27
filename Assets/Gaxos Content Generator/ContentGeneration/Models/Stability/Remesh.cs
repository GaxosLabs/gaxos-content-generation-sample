using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Stability
{
    public enum Remesh
    {
        None, Triangle
    }

    internal class RemeshConverter : EnumJsonConverter<Remesh>
    {
        public override void WriteJson(JsonWriter writer, Remesh value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().ToLowerInvariant());
        }
    }
}
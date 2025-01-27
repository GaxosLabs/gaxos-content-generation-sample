using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Meshy
{
    public enum SurfaceMode
    {
        Organic, Hard
    }

    internal class SurfaceModeConverter : EnumJsonConverter<SurfaceMode>
    {
        public override void WriteJson(JsonWriter writer, SurfaceMode value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().ToLowerInvariant());
        }
    }
}
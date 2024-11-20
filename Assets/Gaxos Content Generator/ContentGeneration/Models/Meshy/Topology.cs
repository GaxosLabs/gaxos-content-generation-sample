using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Meshy
{
    public enum Topology
    {
        Quad, Triangle
    }

    internal class TopologyConverter : EnumJsonConverter<Topology>
    {
        public override void WriteJson(JsonWriter writer, Topology value, JsonSerializer serializer)
        {
            var str = value.ToString().ToLowerInvariant();
            writer.WriteValue(str);
        }

        protected override string AdaptString(string str)
        {
            return base.AdaptString(str).Replace("-", "");
        }
    }
}
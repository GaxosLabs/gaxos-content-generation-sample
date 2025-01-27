using Newtonsoft.Json;

namespace ContentGeneration.Models.Gaxos
{
    public record GaxosTextToImageParameters : GaxosParameters
    {
        [JsonProperty("width")] public uint Width = 512;
        [JsonProperty("height")] public uint Height = 512;
    }
}
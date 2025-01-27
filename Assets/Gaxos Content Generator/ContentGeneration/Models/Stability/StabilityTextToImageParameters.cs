using Newtonsoft.Json;

namespace ContentGeneration.Models.Stability
{
    public record StabilityTextToImageParameters : StabilityParameters
    {
        [JsonProperty("engine_id")] public string EngineId = "stable-diffusion-v1-6";

        [JsonProperty("width")] public uint Width = 512;
        [JsonProperty("height")] public uint Height = 512;
    }
}
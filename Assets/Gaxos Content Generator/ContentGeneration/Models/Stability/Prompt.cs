using Newtonsoft.Json;

namespace ContentGeneration.Models.Stability
{
    public record Prompt
    {
        [JsonProperty("text")] public string Text;
        [JsonProperty("weight")] public float Weight;
    }
}
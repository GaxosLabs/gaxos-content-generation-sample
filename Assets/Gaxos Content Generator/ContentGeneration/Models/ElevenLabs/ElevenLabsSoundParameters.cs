using Newtonsoft.Json;

namespace ContentGeneration.Models.ElevenLabs
{
    public record ElevenLabsSoundParameters
    {
        [JsonProperty("text")] public string Text;
        [JsonProperty("duration_seconds", NullValueHandling = NullValueHandling.Ignore)] public float? DurationSeconds;
        [JsonProperty("prompt_influence")] public float PromptInfluence;
    }
}
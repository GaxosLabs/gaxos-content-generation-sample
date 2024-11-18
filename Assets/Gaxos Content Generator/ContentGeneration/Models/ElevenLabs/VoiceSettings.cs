using Newtonsoft.Json;

namespace ContentGeneration.Models.ElevenLabs
{
    public record VoiceSettings
    {
        [JsonProperty("stability")] public float Stability;
        [JsonProperty("similarity_boost")] public float SimilarityBoost;
        [JsonProperty("style")] public float Style;
        [JsonProperty("use_speaker_boost")] public bool UseSpeakerBoost = true;
    }
}
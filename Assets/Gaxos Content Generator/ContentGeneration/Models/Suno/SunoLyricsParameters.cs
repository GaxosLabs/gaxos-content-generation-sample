using Newtonsoft.Json;

namespace ContentGeneration.Models.Suno
{
    public record SunoLyricsParameters
    {
        [JsonProperty("prompt")] public string Prompt;
    }
}
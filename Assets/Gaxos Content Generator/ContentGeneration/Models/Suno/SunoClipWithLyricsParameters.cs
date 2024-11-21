using Newtonsoft.Json;

namespace ContentGeneration.Models.Suno
{
    public record SunoClipWithLyricsParameters
    {
        [JsonProperty("prompt")] public string Prompt;
        [JsonProperty("tags")] public string Tags;
        [JsonProperty("title")] public string Title;
    }
}
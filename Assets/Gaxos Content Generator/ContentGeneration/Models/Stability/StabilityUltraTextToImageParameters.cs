using Newtonsoft.Json;

namespace ContentGeneration.Models.Stability
{
    public record StabilityUltraTextToImageParameters
    {
        [JsonProperty("prompt")] public string Prompt;
        [JsonProperty("aspect_ratio"), JsonConverter(typeof(AspectRatioConverter))] 
        public AspectRatio AspectRatio = AspectRatio._1_1;
        [JsonProperty("negative_prompt")] public string NegativePrompt;
        [JsonProperty("seed")] public ulong Seed;
        [JsonProperty("output_format"), JsonConverter(typeof(OutputFormatConverter))] public OutputFormat OutputFormat = OutputFormat.Png;
    }
}
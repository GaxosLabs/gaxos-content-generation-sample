using Newtonsoft.Json;

namespace ContentGeneration.Models.Stability
{
    public record StabilityParameters
    {
        [JsonProperty("text_prompts")] public Prompt[] TextPrompts;
        [JsonProperty("cfg_scale")] public uint CfgScale = 7;

        [JsonProperty("clip_guidance_preset"), JsonConverter(typeof(ClipGuidancePresetConverter))]
        public ClipGuidancePreset ClipGuidancePreset = ClipGuidancePreset.None;

        [JsonProperty("sampler", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(SamplerConverter))]
        public Sampler? Sampler;

        [JsonProperty("samples")] public uint Samples = 1;
        [JsonProperty("seed")] public ulong Seed;
        [JsonProperty("steps")] public uint Steps = 30;

        [JsonProperty("style_preset")]
        public string StylePreset;

        [JsonProperty("extras")] public object Extras;
    }
}
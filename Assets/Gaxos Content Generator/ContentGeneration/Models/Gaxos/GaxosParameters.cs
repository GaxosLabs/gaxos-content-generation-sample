using Newtonsoft.Json;

namespace ContentGeneration.Models.Gaxos
{
    public abstract record GaxosParameters
    {
        [JsonProperty("prompt")] public string Prompt;
        [JsonProperty("negative_prompt")] public string NegativePrompt;

        [JsonProperty("checkpoint", NullValueHandling = NullValueHandling.Ignore)] public string Checkpoint;
        [JsonProperty("n_samples")] public uint NSamples = 1;
        [JsonProperty("seed", NullValueHandling = NullValueHandling.Ignore)] public long? Seed;
        [JsonProperty("steps")] public uint Steps = 25;
        [JsonProperty("cfg")] public float Cfg = 6.0f;
        [JsonProperty("sampler_name", NullValueHandling = NullValueHandling.Ignore)] public string SamplerName;
        [JsonProperty("scheduler", NullValueHandling = NullValueHandling.Ignore)] public string Scheduler;
        [JsonProperty("denoise", NullValueHandling = NullValueHandling.Ignore)] public float? Denoise;
        [JsonProperty("loras", NullValueHandling = NullValueHandling.Ignore)] public string[] Loras;
    }
}
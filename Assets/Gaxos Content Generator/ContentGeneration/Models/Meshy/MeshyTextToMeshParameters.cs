using Newtonsoft.Json;

namespace ContentGeneration.Models.Meshy
{
    public record MeshyTextToMeshParameters
    {
        [JsonProperty("prompt")] public string Prompt;

        [JsonProperty("mode")] string mode => "preview";

        [JsonProperty("negative_prompt", NullValueHandling = NullValueHandling.Ignore)] 
        public string NegativePrompt;

        [JsonProperty("art_style"), JsonConverter(typeof(TextToMeshArtStyleConverter))] 
        public TextToMeshArtStyle ArtStyle = TextToMeshArtStyle.Realistic;

        [JsonProperty("seed", NullValueHandling = NullValueHandling.Ignore)] public int? Seed;

        [JsonProperty("ai_model"), JsonConverter(typeof(AiModelConverter))] 
        public AiModel AIModel;

        [JsonProperty("topology"), JsonConverter(typeof(TopologyConverter))] 
        public Topology Topology;

        [JsonProperty("target_polycount")] public int TargetPolyCount = 30000;
    }
}
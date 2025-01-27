using Newtonsoft.Json;

namespace ContentGeneration.Models.Meshy
{
    public record MeshyTextToVoxelParameters
    {
        [JsonProperty("prompt")] public string Prompt;

        [JsonProperty("negative_prompt", NullValueHandling = NullValueHandling.Ignore)] 
        public string NegativePrompt;

        [JsonProperty("voxel_size_shrink_factor"), JsonConverter(typeof(VoxelSizeShrinkFactorConverter))] 
        public VoxelSizeShrinkFactor VoxelSizeShrinkFactor = VoxelSizeShrinkFactor._1;

        [JsonProperty("seed", DefaultValueHandling = DefaultValueHandling.Ignore)] public int? Seed;
    }
}
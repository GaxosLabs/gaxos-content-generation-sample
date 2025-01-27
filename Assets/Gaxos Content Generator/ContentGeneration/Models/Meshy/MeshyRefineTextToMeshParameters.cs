using Newtonsoft.Json;

namespace ContentGeneration.Models.Meshy
{
    public record MeshyRefineTextToMeshParameters
    {
        [JsonProperty("texture_richness", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(TextureRichnessConverter))]
        public TextureRichness TextureRichness;
    }
}
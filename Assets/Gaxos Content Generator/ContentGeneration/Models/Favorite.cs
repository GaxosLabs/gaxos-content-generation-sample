using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContentGeneration.Models
{
    public record Favorite
    {
        [JsonProperty("data")]
        public JObject Data;
        [JsonProperty("generator"), JsonConverter(typeof(GeneratorTypeConverter))]
        public Generator Generator;
        [JsonProperty("generator_parameters")]
        public JObject GeneratorParameters;
        [JsonProperty("id")]
        public string ID;

        [JsonProperty("options")]
        public GenerationOptions GenerationOptions;

        [JsonProperty("deducted_credits")] 
        public float DeductedCredits;
    }
}
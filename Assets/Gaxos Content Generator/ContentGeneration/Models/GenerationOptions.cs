using ContentGeneration.Models.Stability;
using Newtonsoft.Json;
using UnityEngine;

namespace ContentGeneration.Models
{
    public record GenerationOptions
    {
        public const GenerationOptions None = null;

        [JsonProperty("transparent_color", NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(ColorConverter))]
        public Color? TransparentColor;

        [JsonProperty("transparent_color_replace_delta")]
        public float TransparentColorReplaceDelta = 20;

        [JsonProperty("improve_prompt")]
        public bool ImprovePrompt;
    }
}
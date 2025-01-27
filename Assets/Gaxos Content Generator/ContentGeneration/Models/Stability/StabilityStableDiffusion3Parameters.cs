using System;
using Newtonsoft.Json;
using UnityEngine;

namespace ContentGeneration.Models.Stability
{
    public record StabilityStableDiffusion3Parameters
    {
        [JsonProperty("prompt")] public string Prompt;

        [JsonProperty("mode"), JsonConverter(typeof(ModeConverter))]
        public Mode Mode = Mode.TextToImage;
        
        [JsonProperty("image")] string _imageBase64;
        [JsonIgnore] Texture2D _image;

        [JsonIgnore]
        public Texture2D Image
        {
            get => _image;
            set
            {
                _image = value;
                if (value == null)
                {
                    _imageBase64 = null;
                }
                else
                {
                    var bytes = _image.EncodeToPNG();
                    _imageBase64 = Convert.ToBase64String(bytes);
                }
            }
        }
        [JsonProperty("strength")] public float Strength;

        [JsonProperty("aspect_ratio", DefaultValueHandling = DefaultValueHandling.Ignore), JsonConverter(typeof(AspectRatioConverter))] 
        public AspectRatio? AspectRatio;
        
        [JsonProperty("model"), JsonConverter(typeof(ModelConverter))]
        public Model Model = Model.Sd3Large;
        
        [JsonProperty("negative_prompt")] public string NegativePrompt;
        [JsonProperty("seed")] public ulong Seed;
        [JsonProperty("output_format"), JsonConverter(typeof(OutputFormatConverter))] public OutputFormat OutputFormat = OutputFormat.Png;
    }
}
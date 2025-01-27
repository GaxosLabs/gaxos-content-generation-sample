using System;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Meshy
{
    public record MeshyTextToTextureParameters
    {
        [JsonProperty("model_extension")] public string ModelExtension;
        [JsonProperty("model")] string _modelBase64;

        [JsonIgnore] byte[] _model;

        [JsonIgnore]
        public byte[] Model
        {
            get => _model;
            set
            {
                _model = value;
                if (value == null)
                {
                    _modelBase64 = null;
                }
                else
                {
                    _modelBase64 = Convert.ToBase64String(_model);
                }
            }
        }

        [JsonProperty("object_prompt")] public string ObjectPrompt;
        [JsonProperty("style_prompt")] public string StylePrompt;
        [JsonProperty("negative_prompt")] public string NegativePrompt;
        [JsonProperty("enable_original_uv")] public bool EnableOriginalUV = true;
        [JsonProperty("enable_pbr")] public bool EnablePbr = true;
        [JsonProperty("resolution"), JsonConverter(typeof(ResolutionConverter))] 
        public Resolution Resolution;

        [JsonProperty("art_style"), JsonConverter(typeof(TextToTextureArtStyleConverter))]
        public TextToTextureArtStyle ArtStyle;
    }
}
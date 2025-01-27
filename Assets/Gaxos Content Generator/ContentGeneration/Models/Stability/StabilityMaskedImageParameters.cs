using System;
using Newtonsoft.Json;
using UnityEngine;

namespace ContentGeneration.Models.Stability
{
    public record StabilityMaskedImageParameters : StabilityParameters
    {
        [JsonProperty("engine_id")] public string EngineId = "stable-diffusion-xl-1024-v1-0";

        [JsonProperty("init_image")] string _initImageBase64;
        [JsonIgnore] Texture2D _initImage;

        [JsonIgnore]
        public Texture2D InitImage
        {
            get => _initImage;
            set
            {
                _initImage = value;
                if (value == null)
                {
                    _initImageBase64 = null;
                }
                else
                {
                    var bytes = _initImage.EncodeToPNG();
                    _initImageBase64 = Convert.ToBase64String(bytes);
                }
            }
        }
        
        
        [JsonProperty("mask_source"), JsonConverter(typeof(MaskSourceConverter))]
        public MaskSource MaskSource = MaskSource.InitImageAlpha;
        
        [JsonProperty("mask_image")] string _maskImageBase64;
        [JsonIgnore] Texture2D _maskImage;

        [JsonIgnore]
        public Texture2D MaskImage
        {
            get => _maskImage;
            set
            {
                _maskImage = value;
                if (value == null)
                {
                    _maskImageBase64 = null;
                }
                else
                {
                    var bytes = _maskImage.EncodeToPNG();
                    _maskImageBase64 = Convert.ToBase64String(bytes);
                }
            }
        }
    }
}
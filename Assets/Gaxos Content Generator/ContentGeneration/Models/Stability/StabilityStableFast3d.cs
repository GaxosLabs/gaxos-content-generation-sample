using System;
using Newtonsoft.Json;
using UnityEngine;

namespace ContentGeneration.Models.Stability
{
    public record StabilityStableFast3d
    {
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
        
        [JsonProperty("texture_resolution"), JsonConverter(typeof(TextureResolutionConverter))] 
        public TextureResolution TextureResolution = TextureResolution._1024;
        [JsonProperty("foreground_ratio")] public float ForegroundRatio = .85f;
        [JsonProperty("remesh"), JsonConverter(typeof(RemeshConverter))] 
        public Remesh Remesh = Remesh.None;
    }
}
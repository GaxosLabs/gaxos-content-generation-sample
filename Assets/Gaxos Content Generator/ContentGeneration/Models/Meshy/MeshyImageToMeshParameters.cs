using System;
using Newtonsoft.Json;
using UnityEngine;

namespace ContentGeneration.Models.Meshy
{
    public record MeshyImageToMeshParameters
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
        
        [JsonProperty("image_extension")] string imageExtension => "png";
        [JsonProperty("enable_pbr")] public bool EnablePbr;
        [JsonProperty("surface_mode"), JsonConverter(typeof(SurfaceModeConverter))] 
        public SurfaceMode SurfaceMode;
    }
}
using System;
using Newtonsoft.Json;
using UnityEngine;

namespace ContentGeneration.Models.DallE
{
    public record DallEInpaintingParameters : DallEParameters
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
        
        
        [JsonProperty("mask")] string _maskBase64;
        [JsonIgnore] Texture2D _mask;

        [JsonIgnore]
        public Texture2D Mask
        {
            get => _mask;
            set
            {
                _mask = value;
                if (value == null)
                {
                    _maskBase64 = null;
                }
                else
                {
                    var bytes = _mask.EncodeToPNG();
                    _maskBase64 = Convert.ToBase64String(bytes);
                }
            }
        }
    }
}
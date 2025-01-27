using System;
using Newtonsoft.Json;
using UnityEngine;

namespace ContentGeneration.Models.Gaxos
{
    public record GaxosMaskingParameters : GaxosParameters
    {
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
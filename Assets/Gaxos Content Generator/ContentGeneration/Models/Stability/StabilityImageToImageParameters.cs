using System;
using ContentGeneration.Helpers;
using Newtonsoft.Json;
using UnityEngine;

namespace ContentGeneration.Models.Stability
{
    public record StabilityImageToImageParameters : StabilityParameters
    {
        [JsonProperty("engineId")] public string EngineId = "stable-diffusion-v1-6";

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

        [JsonProperty("init_image_mode"), JsonConverter(typeof(InitImageModeConverter))]
        public InitImageMode InitImageMode = InitImageMode.ImageStrength;

        [JsonProperty("step_schedule_start", NullValueHandling = NullValueHandling.Ignore)] public float? StepScheduleStart;
        [JsonProperty("step_schedule_end", NullValueHandling = NullValueHandling.Ignore)] public float? StepScheduleEnd;
        [JsonProperty("image_strength", NullValueHandling = NullValueHandling.Ignore)] public float? ImageStrength = .35f;
    }

    public enum InitImageMode
    {
        ImageStrength,
        StepSchedule
    }

    internal class InitImageModeConverter : EnumJsonConverter<InitImageMode>
    {
        public override void WriteJson(JsonWriter writer, InitImageMode value, JsonSerializer serializer)
        {
            var str = value switch
            {
                InitImageMode.ImageStrength => "IMAGE_STRENGTH",
                InitImageMode.StepSchedule => "STEP_SCHEDULE",
                _ => value.ToString().ToUpperInvariant(),
            };
            writer.WriteValue(str);
        }

        protected override string AdaptString(string str)
        {
            return base.AdaptString(str).Replace("_", "");
        }
    }
}
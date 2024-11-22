using System;
using Newtonsoft.Json;
using UnityEngine;

namespace ContentGeneration.Models.Recraft
{
    public class ColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            var val = ColorUtility.ToHtmlStringRGB(value);
            writer.WriteValue(new[]
            {
                Mathf.RoundToInt(value.r * 255),
                Mathf.RoundToInt(value.g * 255),
                Mathf.RoundToInt(value.b * 255)
            });
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            try
            {
                var value = (int[])reader.Value!;
                return new Color(
                    value[0] / 255f,
                    value[1] / 255f,
                    value[2] / 255f
                );
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse color {objectType} : {ex.Message}");
                return Color.black;
            }
        }
    }
}
using System;
using Newtonsoft.Json;
using UnityEngine;

namespace ContentGeneration.Models.Stability
{
    public class ColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            var val = ColorUtility.ToHtmlStringRGB(value);
            writer.WriteValue(val);
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            try
            {
                ColorUtility.TryParseHtmlString("#" + reader.Value, out var loadedColor);
                return loadedColor;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse color {objectType} : {ex.Message}");
                return Color.black; 
            }
        }
    }
}
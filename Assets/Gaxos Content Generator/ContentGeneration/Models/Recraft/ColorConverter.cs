using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ContentGeneration.Models.Recraft
{
    public class ColorConverter : JsonConverter<Color?>
    {
        public override void WriteJson(JsonWriter writer, Color? value, JsonSerializer serializer)
        {
            if (!value.HasValue)
            {
                serializer.Serialize(writer, null);
                return;
            }
            
            serializer.Serialize(writer, new
            {
                rgb= new[]
                {
                    Mathf.RoundToInt(value.Value.r * 255),
                    Mathf.RoundToInt(value.Value.g * 255),
                    Mathf.RoundToInt(value.Value.b * 255)
                } 
            });
        }

        public override Color? ReadJson(JsonReader reader, Type objectType, Color? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            try
            {
                if (reader.TokenType == JsonToken.Null)
                {
                    return null;
                }
                var value = ((JTokenReader)reader)!.CurrentToken!["rgb"]!.ToArray().Select(i => i.ToObject<int>()).ToArray();
                reader.Skip();
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
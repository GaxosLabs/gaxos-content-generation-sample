using System;
using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Recraft
{
    internal class SubstyleConverter : JsonConverter<string>
    {
        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
        {
            writer.WriteValue(string.IsNullOrEmpty(value) ? "" : value.CamelCaseToUnderscores());
        }

        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var ret = "";

            var makeUpper = true;
            for (var i = 0; i < existingValue.Length; i++)
            {
                if (existingValue[i] == '_')
                {
                    makeUpper = true;
                }
                else
                {
                    if (makeUpper)
                    {
                        ret += char.ToUpper(existingValue[i]);
                    }
                    else
                    {
                        ret += existingValue[i];
                    }

                    makeUpper = false;
                }
            }

            return ret;
        }
    }
}
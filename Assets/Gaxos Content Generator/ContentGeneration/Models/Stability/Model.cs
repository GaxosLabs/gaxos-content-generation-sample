using System;
using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Stability
{
    public enum Model
    {
        Sd3Large, Sd3LargeTurbo, Sd3Medium
    }

    internal class ModelConverter : EnumJsonConverter<Model>
    {
        public override void WriteJson(JsonWriter writer, Model value, JsonSerializer serializer)
        {
            var str = value switch
            {
                Model.Sd3Large => "sd3-large",
                Model.Sd3LargeTurbo => "sd3-large-turbo",
                Model.Sd3Medium => "sd3-medium",
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
            writer.WriteValue(str);
        }

        protected override string AdaptString(string str)
        {
            return str.DashesToCamelCase();
        }
    }
}
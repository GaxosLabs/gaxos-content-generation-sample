using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Meshy
{
    public enum AiModel
    {
        Meshy4, Meshy3Turbo, Meshy3
    }

    internal class AiModelConverter : EnumJsonConverter<AiModel>
    {
        public override void WriteJson(JsonWriter writer, AiModel value, JsonSerializer serializer)
        {
            var str = value switch
            {
                AiModel.Meshy4 => "meshy-4",
                AiModel.Meshy3Turbo => "meshy-3-turbo",
                AiModel.Meshy3 => "meshy-3",
                _ => value.ToString().ToLowerInvariant(),
            };
            writer.WriteValue(str);
        }

        protected override string AdaptString(string str)
        {
            return base.AdaptString(str).Replace("-", "");
        }
    }
}
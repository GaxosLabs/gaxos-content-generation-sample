using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Stability
{
    public enum ClipGuidancePreset
    {
        None, FastBlue, FastGreen, Simple, Slow, Slower, Slowest
    }

    internal class ClipGuidancePresetConverter : EnumJsonConverter<ClipGuidancePreset>
    {
        public override void WriteJson(JsonWriter writer, ClipGuidancePreset value, JsonSerializer serializer)
        {
            var str = value switch
            {
                ClipGuidancePreset.FastBlue => "FAST_BLUE",
                ClipGuidancePreset.FastGreen => "FAST_GREEN",
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
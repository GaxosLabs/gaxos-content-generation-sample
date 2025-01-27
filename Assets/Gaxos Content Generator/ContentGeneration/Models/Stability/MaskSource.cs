using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Stability
{
    public enum MaskSource
    {
        MaskImageWhite, MaskImageBlack, InitImageAlpha
    }

    internal class MaskSourceConverter : EnumJsonConverter<MaskSource>
    {
        public override void WriteJson(JsonWriter writer, MaskSource value, JsonSerializer serializer)
        {
            var str = value switch
            {
                MaskSource.InitImageAlpha => "INIT_IMAGE_ALPHA",
                MaskSource.MaskImageWhite => "MASK_IMAGE_WHITE",
                MaskSource.MaskImageBlack => "MASK_IMAGE_BLACK",
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
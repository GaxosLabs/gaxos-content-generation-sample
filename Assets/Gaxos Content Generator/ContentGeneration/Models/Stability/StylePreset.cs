using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Stability
{
    public enum StylePreset
    {
        ThreeDModel,
        AnalogFilm,
        Anime,
        Cinematic,
        ComicBook,
        DigitalArt,
        Enhance,
        FantasyArt,
        Isometric,
        LineArt,
        LowPoly,
        ModelingCompound,
        NeonPunk,
        Origami,
        Photographic,
        PixelArt,
        TileTexture
    }

    internal class StylePresetConverter : EnumJsonConverter<StylePreset>
    {
        public override void WriteJson(JsonWriter writer, StylePreset value, JsonSerializer serializer)
        {
            var str = value switch
            {
                StylePreset.ThreeDModel => "3d-model",
                _ => value.ToString().CamelCaseToDashes()
            };
            writer.WriteValue(str);
        }

        protected override string AdaptString(string str)
        {
            if (str == "3d-model")
            {
                return StylePreset.ThreeDModel.ToString();
            }
            return str.DashesToCamelCase();
        }
    }
}
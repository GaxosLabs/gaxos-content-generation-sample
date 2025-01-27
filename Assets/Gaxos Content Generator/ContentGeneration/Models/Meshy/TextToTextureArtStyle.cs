using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.Meshy
{
    public enum TextToTextureArtStyle
    {
        Realistic,
        Fake3dCartoon,
        JapaneseAnime,
        CartoonLineArt,
        RealisticHandDrawn,
        Fake3dHandDrawn,
        OrientalComicInk,
    }

    internal class TextToTextureArtStyleConverter : EnumJsonConverter<TextToTextureArtStyle>
    {
        public override void WriteJson(JsonWriter writer, TextToTextureArtStyle value, JsonSerializer serializer)
        {
            var str = value switch
            {
                TextToTextureArtStyle.Realistic => "realistic",
                TextToTextureArtStyle.Fake3dCartoon => "fake-3d-cartoon",
                TextToTextureArtStyle.JapaneseAnime => "japanese-anime",
                TextToTextureArtStyle.CartoonLineArt => "cartoon-line-art",
                TextToTextureArtStyle.RealisticHandDrawn => "realistic-hand-drawn",
                TextToTextureArtStyle.Fake3dHandDrawn => "fake-3d-hand-drawn",
                TextToTextureArtStyle.OrientalComicInk => "oriental-comic-ink",
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
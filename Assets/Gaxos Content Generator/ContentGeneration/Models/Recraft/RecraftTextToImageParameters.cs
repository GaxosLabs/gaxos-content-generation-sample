using System;
using Newtonsoft.Json;
using UnityEngine;

namespace ContentGeneration.Models.Recraft
{
    public record RecraftTextToImageParameters
    {
        [JsonProperty("prompt")] public string Prompt;
        [JsonProperty("n")] public int N = 1;
        [JsonProperty("style"), JsonConverter(typeof(StyleConverter))] public Style Style = Style.RealisticImage;
        [JsonProperty("substyle")] public string Substyle;
        [JsonProperty("model"), JsonConverter(typeof(ModelConverter))] public Model Model = Model.Recraftv3;
        [JsonProperty("size"), JsonConverter(typeof(SizeConverter))] public Size Size = Size._1024x1024;
        [JsonProperty("controls", NullValueHandling = NullValueHandling.Ignore)] public Controls Controls;
    }

    public record Controls
    {
        [JsonProperty("colors", ItemConverterType = typeof(ColorConverter))] public Color[] Colors = Array.Empty<Color>();
        [JsonProperty("background_color"), JsonConverter(typeof(ColorConverter))] public Color? BackgroundColor;
    } 
}
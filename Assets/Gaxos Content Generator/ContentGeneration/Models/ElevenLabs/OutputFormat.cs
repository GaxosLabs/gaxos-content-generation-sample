using System.Diagnostics.CodeAnalysis;
using ContentGeneration.Helpers;
using Newtonsoft.Json;

namespace ContentGeneration.Models.ElevenLabs
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum OutputFormat
    {
        Mp3_22050_32,
        Mp3_44100_32,
        Mp3_44100_64,
        Mp3_44100_96,
        Mp3_44100_128,
        Mp3_44100_192,
        Pcm_16000,
        Pcm_22050,
        Pcm_24000,
        Pcm_44100,
        Ulaw_8000 
    }
    internal class OutputFormatConverter : EnumJsonConverter<OutputFormat>
    {
        public override void WriteJson(JsonWriter writer, OutputFormat value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().ToLowerInvariant());
        }
    }
}
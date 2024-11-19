using Newtonsoft.Json;

namespace ContentGeneration.Models.ElevenLabs
{
    public record ElevenLabsTextToSpeechParameters
    {
        [JsonProperty("text")] public string Text;
        [JsonProperty("voice_id")] public string VoiceID;
        [JsonProperty("output_format"), JsonConverter(typeof(OutputFormatConverter))] public OutputFormat OutputFormat = OutputFormat.Mp3_44100_128;

        [JsonProperty("model_id")] public string ModelID = "eleven_monolingual_v1";
        [JsonProperty("language_code")] public string LanguageCode;

        [JsonProperty("voice_settings", NullValueHandling = NullValueHandling.Ignore)] public VoiceSettings VoiceSettings;

        [JsonProperty("pronunciation_dictionary_locators")] public PronunciationDictionaryLocator[] PronunciationDictionaryLocators;
        [JsonProperty("seed")] public int Seed;
        [JsonProperty("previous_text")] public string PreviousText;
        [JsonProperty("next_text")] public string NextText;
        [JsonProperty("previous_request_ids")] public string[] PreviousRequestIds;
        [JsonProperty("next_request_ids")] public string[] NextRequestIds;
        [JsonProperty("apply_text_normalization"), JsonConverter(typeof(TextNormalizationConverter))] public TextNormalization ApplyTextNormalization;
    }
}
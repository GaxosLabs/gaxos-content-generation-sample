using Newtonsoft.Json;

namespace ContentGeneration.Models.ElevenLabs
{
    public record PronunciationDictionaryLocator
    {
        [JsonProperty("pronunciation_dictionary_id")] public string PronunciationDictionaryID;
        [JsonProperty("version_id")] public string VersionID;
    }
}
using Newtonsoft.Json;

namespace ContentGeneration.Models.Suno
{
    public record SunoClipParameters
    {
        [JsonProperty("gpt_description_prompt")] public string GptDescriptionPrompt;
        [JsonProperty("mv"), JsonConverter(typeof(ModelConverter))] public Model Mv = Model.ChirpV35;
        [JsonProperty("make_instrumental")] public bool MakeInstrumental;
    }
}
using Newtonsoft.Json;

namespace ContentGeneration.Models.DallE
{
    public abstract record DallEParameters
    {
        [JsonProperty("prompt")] public string Prompt;
        [JsonProperty("model"), JsonConverter(typeof(ModelConverter))] public Model Model;

        [JsonProperty("n")] public uint N = 1;
        [JsonProperty("quality", NullValueHandling = NullValueHandling.Ignore), 
         JsonConverter(typeof(QualityConverter))] 
        public Quality? Quality;
        [JsonProperty("size")] string _size = "1024x1024";

        [JsonIgnore]
        public uint Width
        {
            get => uint.Parse(_size.Split('x')[0]);
            set => _size = $"{value}x{Height}";
        }
        [JsonIgnore]
        public uint Height
        {
            get => uint.Parse(_size.Split('x')[1]);
            set => _size = $"{Width}x{value}";
        }
        [JsonProperty("style", NullValueHandling = NullValueHandling.Ignore), 
         JsonConverter(typeof(StyleConverter))] 
        public Style? Style;
    }
}
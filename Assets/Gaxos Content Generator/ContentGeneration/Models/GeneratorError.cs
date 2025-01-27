using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContentGeneration.Models
{
    public record GeneratorError
    {
        [JsonProperty("message")]
        public string Message;
        [JsonProperty("error")]
        public JObject Error;
    }
}
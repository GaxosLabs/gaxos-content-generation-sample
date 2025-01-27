using System;
using Newtonsoft.Json;

namespace ContentGeneration.Models
{
    public record PublishedAsset
    {
        [JsonProperty("id")]
        public string ID;

        [JsonProperty("s3_url")]
        public string URL;

        [JsonProperty("created_at"), JsonConverter(typeof(DateTimeFromUnixTimeStampConverter))]
        public DateTime CreatedAt;

        [JsonProperty("request")]
        public Request Request;
    }
}
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContentGeneration.Models
{
    public record Request
    {
        [JsonProperty("completed_at"), JsonConverter(typeof(DateTimeFromUnixTimeStampConverter))]
        public DateTime CompletedAt;
        [JsonProperty("created_at"), JsonConverter(typeof(DateTimeFromUnixTimeStampConverter))]
        public DateTime CreatedAt;

        [JsonProperty("data")]
        public JObject Data;
        [JsonProperty("generator"), JsonConverter(typeof(GeneratorTypeConverter))]
        public Generator Generator;
        [JsonProperty("generator_parameters")]
        public JObject GeneratorParameters;
        [JsonProperty("id")]
        public string ID;
        [JsonProperty("status"), JsonConverter(typeof(RequestStatusConverter))]
        public RequestStatus Status;

        [JsonProperty("generator_error")]
        public GeneratorError GeneratorError;

        [JsonProperty("assets")]
        public GeneratedAsset[] Assets;

        [JsonProperty("generator_result")] 
        public JObject GeneratorResult;

        [JsonProperty("deducted_topup_credits")] 
        public float DeductedTopupCredits;

        [JsonProperty("deducted_subscription_credits")] 
        public float DeductedSubscriptionCredits;
    }
}
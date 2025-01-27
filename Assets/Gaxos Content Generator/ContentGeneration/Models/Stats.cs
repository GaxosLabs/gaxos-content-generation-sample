using Newtonsoft.Json;

namespace ContentGeneration.Models
{
    public record Stats
    {
        public record StatsItem<T>
        {
            [JsonProperty("used")]
            public T Used;
            [JsonProperty("total")]
            public T Total;
        }
        public record StatsCredits : StatsItem<float>
        {
            public override string ToString()
            {
                return $"{Total - Used:0.##} / {Total:0.##}";
            }
        }
        public record StatsStorage : StatsItem<ulong>
        {
            public override string ToString()
            {
                return $"{(Total - Used) / (1024 * 1024)} / {Total / (1024 * 1024)} MB";
            }
        }
        public record StatsRequests : StatsItem<ulong>
        {
            public override string ToString()
            {
                return $"{Total - Used} / {Total}";
            }
        }

        [JsonProperty("credits")] public StatsCredits Credits;
        [JsonProperty("storage")] public StatsStorage Storage;
        [JsonProperty("requests")] public StatsRequests Requests;
    }
}
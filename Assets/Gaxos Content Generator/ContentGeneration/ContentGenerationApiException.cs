using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace ContentGeneration
{
    public class ContentGenerationApiException : Exception
    {
        public readonly UnityWebRequest www;
        readonly string message;

        public ContentGenerationApiException(
            UnityWebRequest www, 
            Dictionary<string, string> headers,
            object data) :
            base($"[{www.method} {www.uri}] {www.error}")
        {
            this.www = www;

            message = GetWwwDetails(www, headers, data);
        }

        internal static string GetWwwDetails(UnityWebRequest www, Dictionary<string, string> headers, object data)
        {
            var message = $"{www.method} {www.uri}";

            message += $"\n=> {www.error}";
            if (!string.IsNullOrEmpty(www.downloadHandler?.text))
            {
                message += $"\n{www.downloadHandler.text}";
            }

            message += $"\n\n";

            if (headers is { Count: > 0 })
            {
                var headersArray = headers.Select(h => $"[{h.Key}={h.Value}]");
                message += $"\nheaders:\n\t{string.Join("\n\t", headersArray)}";
            }
            if (data != null)
            {
                message += $"\ndata: {JsonConvert.SerializeObject(data)}";
            }
            
            return message;
        }
        
        internal static string GetRequestDetails(ContentGenerationApi.ApiMethod method, string endpoint, Dictionary<string, string> headers, object data)
        {
            var message = $"{method} {endpoint}";

            if (headers is { Count: > 0 })
            {
                var headersArray = headers.Select(h => $"[{h.Key}={h.Value}]");
                message += $"\nheaders:\n\t{string.Join("\n\t", headersArray)}";
            }
            if (data != null)
            {
                message += $"\ndata: {JsonConvert.SerializeObject(data)}";
            }
            
            return message;
        }

        public override string Message => message;
    }
}
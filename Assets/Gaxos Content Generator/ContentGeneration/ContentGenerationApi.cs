using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ContentGeneration.Models;
using ContentGeneration.Models.DallE;
using ContentGeneration.Models.ElevenLabs;
using ContentGeneration.Models.Gaxos;
using ContentGeneration.Models.Meshy;
using ContentGeneration.Models.Recraft;
using ContentGeneration.Models.Stability;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using QueryParameters = ContentGeneration.Models.QueryParameters;

namespace ContentGeneration
{
    public class ContentGenerationApi
    {
        public static readonly ContentGenerationApi Instance = new();

        internal enum ApiMethod
        {
            Get,
            Post,
            Delete,
            Patch
        }

        // const string BaseUrl = "https://content-generation-21ab4.web.app/";
        // const string BaseUrl = "http://localhost:5002/";
#if USE_GAXOS_DEV_BACKEND
        const string BaseUrl = "https://dev.gaxoslabs.ai/api/connect/v1/";
#else
        const string BaseUrl = "https://gaxoslabs.ai/api/connect/v1/";
#endif

        async Task<T> SendRequest<T>(ApiMethod method, string endpoint,
            Dictionary<string, string> headers = null,
            object data = null)
        {
            var json = await SendRequest(method, endpoint, headers, data);
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (JsonSerializationException e)
            {
                Debug.LogError(
                    $"Error deserializing JSON ({e.Message}): \n\n" +
                    ContentGenerationApiException.GetRequestDetails(method, endpoint, headers, data) + "\n\n" +
                    json);
                throw;
            }
        }

        Task<string> SendRequest(ApiMethod method, string endpoint,
            Dictionary<string, string> headers = null,
            object data = null)
        {
            var ret = new TaskCompletionSource<string>();
            Dispatcher.instance.StartCoroutine(SendRequestCo(method, endpoint, headers, data, ret));
            return ret.Task;
        }

        IEnumerator SendRequestCo(ApiMethod method, string endpoint, Dictionary<string, string> headers, object data,
            TaskCompletionSource<string> ret)
        {
            var url = BaseUrl + endpoint.TrimStart('/');
            if (data != null && method == ApiMethod.Get)
            {
                var parametersStr = new StringBuilder();
                var properties = data.GetType().GetProperties();
                if (properties.Length > 0)
                {
                    foreach (var property in properties)
                    {
                        parametersStr.Append(
                            $"&{UnityWebRequest.EscapeURL(property.Name)}={UnityWebRequest.EscapeURL(property.GetValue(data)?.ToString())}");
                    }
                }

                parametersStr[0] = '?';
                url += parametersStr;
            }

            var www = new UnityWebRequest(url, method.ToString().ToUpperInvariant());
            www.downloadHandler = new DownloadHandlerBuffer();

            headers ??= new Dictionary<string, string>();
            headers.Add("Authorization", $"Bearer {Settings.instance.apiKey}");

            foreach (var header in headers)
            {
                www.SetRequestHeader(header.Key, header.Value);
            }

            var contentData = Array.Empty<byte>();
            if (data != null && method != ApiMethod.Get)
            {
                var dataJson = JsonConvert.SerializeObject(data,
                    Formatting.None,
                    new GeneratorTypeConverter());
                contentData = Encoding.UTF8.GetBytes(dataJson);
            }

            www.uploadHandler = new UploadHandlerRaw(contentData);
            www.uploadHandler.contentType = "application/json";

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                ret.SetException(new ContentGenerationApiException(www, headers, data));
                yield break;
            }

            // Debug.LogWarning(ContentGenerationApiException.GetWwwDetails(www, headers, data));

            ret.SetResult(www.downloadHandler.text);
        }

        public async Task<Stats> GetStats()
        {
            return await SendRequest<Stats>(ApiMethod.Get, "stats");
        }

        public async Task<Request[]> GetRequests(QueryParameters queryParameters = null)
        {
            var queryParametersStr = GetQueryParametersStr(queryParameters);
            return await SendRequest<Request[]>(ApiMethod.Get, "request" + queryParametersStr);
        }

        public async Task AddFavorite(string requestId)
        {
            await SendRequest(ApiMethod.Post, $"request/favorite/{requestId}");
        }

        public async Task<Favorite[]> GetFavorites()
        {
            return await SendRequest<Favorite[]>(ApiMethod.Get, "request/favorite");
        }

        public async Task DeleteFavorite(string requestId)
        {
            await SendRequest(ApiMethod.Delete, $"request/favorite/{requestId}");
        }


        static string GetQueryParametersStr(QueryParameters queryParameters)
        {
            string queryParametersStr = null;
            if (queryParameters != null)
            {
                queryParametersStr = $"?limit={queryParameters.Limit}&offset={queryParameters.Offset}";
                if (queryParameters.Sort is { Length: > 0 })
                {
                    queryParametersStr += "&sortBy=" + string.Join("&sortBy=", queryParameters.Sort.Select(i =>
                    {
                        var targetStr = i.Target switch
                        {
                            QueryParameters.SortTarget.Id => "id",
                            QueryParameters.SortTarget.CreatedAt => "created_at",
                            _ => i.Target.ToString().ToLowerInvariant()
                        };
                        var directionStr = i.Direction switch
                        {
                            QueryParameters.SortDirection.Ascending => "asc",
                            QueryParameters.SortDirection.Descending => "desc",
                            _ => i.Direction.ToString().ToLowerInvariant()
                        };
                        return $"{targetStr}.{directionStr}";
                    }));
                }

                if (!string.IsNullOrEmpty(queryParameters.FilterByPlayerId))
                {
                    queryParametersStr += $"&filter=player_id.{queryParameters.FilterByPlayerId}";
                }

                if (!string.IsNullOrEmpty(queryParameters.FilterByAssetType))
                {
                    queryParametersStr += $"&filter=asset_type.{queryParameters.FilterByAssetType}";
                }
            }

            return queryParametersStr;
        }

        public async Task<Request> GetRequest(string id)
        {
            return await SendRequest<Request>(ApiMethod.Get, $"request/{id}");
        }

        public async Task<PublishedAsset[]> DeleteRequest(string id)
        {
            return await SendRequest<PublishedAsset[]>(ApiMethod.Delete,
                $"request/{id}");
        }

        public Task<string> RequestGeneration(
            Generator generator,
            object generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return SendRequest(ApiMethod.Post,
                "request",
                data: new
                {
                    data,
                    generator = GeneratorTypeConverter.ToString(generator),
                    generator_parameters = generatorParameters,
                    options
                });
        }

        public Task<string> RequestStabilityTextToImageGeneration(
            StabilityTextToImageParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.StabilityTextToImage,
                generatorParameters, options, data);
        }

        public Task<string> RequestStabilityCoreTextToImageGeneration(
            StabilityCoreTextToImageParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.StabilityTextToImageCore,
                generatorParameters, options, data);
        }

        public Task<string> RequestStabilityUltraTextToImageGeneration(
            StabilityUltraTextToImageParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.StabilityTextToImageUltra,
                generatorParameters, options, data);
        }

        public Task<string> RequestStabilityStableDiffusion3Generation(
            StabilityStableDiffusion3Parameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.StabilityDiffusion3,
                generatorParameters, options, data);
        }

        public Task<string> RequestStabilityStableFast3dGeneration(
            StabilityStableFast3d generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.StabilityStableFast3d,
                generatorParameters, options, data);
        }

        public Task<string> RequestStabilityImageToImageGeneration(
            StabilityImageToImageParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.StabilityImageToImage,
                generatorParameters, options, data);
        }

        public Task<string> RequestStabilityMaskedImageGeneration(
            StabilityMaskedImageParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.StabilityMasking,
                generatorParameters, options, data);
        }

        public Task<string> RequestDallETextToImageGeneration(
            DallETextToImageParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.DallETextToImage,
                generatorParameters, options, data);
        }


        public Task<string> RequestDallEInpaintingGeneration(
            DallEInpaintingParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.DallEInpainting,
                generatorParameters, options, data);
        }

        public async Task MakeAssetPublic(string requestId, string assetId, bool makeItPublic)
        {
            await SendRequest(ApiMethod.Patch,
                $"request/{requestId}/{(makeItPublic ? "publish" : "unpublish")}/{assetId}");
        }

        public async Task<PublishedAsset[]> GetPublishedAssets(QueryParameters queryParameters = null)
        {
            var queryParametersStr = GetQueryParametersStr(queryParameters);
            return await SendRequest<PublishedAsset[]>(ApiMethod.Get, "asset" + queryParametersStr);
        }

        public async Task<PublishedAsset> GetPublishedAsset(string publishedAssetId)
        {
            return await SendRequest<PublishedAsset>(ApiMethod.Get, $"asset/{publishedAssetId}");
        }

        public async Task<string> ImprovePrompt(string prompt, string generator)
        {
            return (await SendRequest(ApiMethod.Get,
                "improve-prompt" +
                $"?generator={WebUtility.UrlDecode(generator)}" +
                $"&prompt={WebUtility.UrlDecode(prompt)}"
            )).Trim('"');
        }

        public Task<string> RequestMeshyTextToMeshGeneration(
            MeshyTextToMeshParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.MeshyTextToMesh,
                generatorParameters, options, data);
        }

        public async Task RefineMeshyTextToMesh(string id, MeshyRefineTextToMeshParameters parameters = null)
        {
            await SendRequest(ApiMethod.Patch, $"request/{id}/meshy-refine-text-to-mesh", data: parameters);
        }

        public Task<string> RequestMeshyTextToTextureGeneration(
            MeshyTextToTextureParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.MeshyTextToTexture,
                generatorParameters, options, data);
        }

        public Task<string> RequestMeshyTextToVoxelGeneration(
            MeshyTextToVoxelParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.MeshyTextToVoxel,
                generatorParameters, options, data);
        }

        public Task<string> RequestMeshyImageToMeshGeneration(
            MeshyImageToMeshParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.MeshyImageTo3d,
                generatorParameters, options, data);
        }

        public Task<string> RequestGaxosTextToImageGeneration(
            GaxosTextToImageParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.GaxosTextToImage,
                generatorParameters, options, data);
        }

        public Task<string> RequestGaxosMaskingGeneration(
            GaxosMaskingParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.GaxosMasking,
                generatorParameters, options, data);
        }

        public Task<string> RequestElevenLabsSoundGeneration(
            ElevenLabsSoundParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.ElevenLabsSound,
                generatorParameters, options, data);
        }

        public Task<string> RequestElevenLabsTextToSpeechGeneration(
            ElevenLabsTextToSpeechParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.ElevenLabsTextToSpeech,
                generatorParameters, options, data);
        }

        public Task<string> RequestRecraftTextToImageGeneration(
            RecraftTextToImageParameters generatorParameters,
            GenerationOptions options = GenerationOptions.None,
            object data = null)
        {
            return RequestGeneration(
                Generator.RecraftTextToImage,
                generatorParameters, options, data);
        }
    }
}
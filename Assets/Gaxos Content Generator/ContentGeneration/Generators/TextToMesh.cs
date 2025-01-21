using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using ContentGeneration.Models.Meshy;
using Dummiesman;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace ContentGeneration.Generators
{
    public class TextToMesh : Generator
    {
        protected override Task<string> RequestGeneration(string prompt)
        {
            return ContentGenerationApi.Instance.RequestMeshyTextToMeshGeneration(new MeshyTextToMeshParameters
            {
                Prompt = prompt,
            });
        }

        [SerializeField] GameObject _modelContainer;
        GameObject _prevLoadedGameObject;

        protected override async Task ReportGeneration(PublishedAsset asset)
        {
            var modelUrl =
                asset.Request.GeneratorResult.ContainsKey("refine_result")
                    ? asset.Request.GeneratorResult["refine_result"]!["model_urls"]!["obj"]!.ToObject<string>()
                    : asset.Request.GeneratorResult["model_urls"]!["obj"]!.ToObject<string>();

            var newObject = await DownloadModel(modelUrl);
            if (_prevLoadedGameObject != null)
            {
                Destroy(_prevLoadedGameObject);
            }

            _prevLoadedGameObject = newObject;
            newObject.transform.SetParent(_modelContainer.transform, false);

            var textures = asset.Request.GeneratorResult.ContainsKey("refine_result")
                ? asset.Request.GeneratorResult["refine_result"]!["texture_urls"]!.ToObject<JArray>()
                : asset.Request.GeneratorResult["texture_urls"]!.ToObject<JArray>();

            if (textures != null)
            {
                foreach (var textureDefinition in textures)
                {
                    if (textureDefinition["base_color"] != null)
                    {
                        var texture =
                            await TextureHelper.DownloadImage(textureDefinition["base_color"]!.ToObject<string>());

                        var material = new Material(Shader.Find("Standard"))
                        {
                            mainTexture = texture
                        };
                        var modelRenderer = newObject.GetComponentInChildren<Renderer>();
                        modelRenderer.material = material;
                        break;
                    }
                }
            }
        }

        protected override async Task RequestWasGenerated(string id, Request result)
        {
            if (!result.GeneratorResult.ContainsKey("refine_status"))
            {
                await ContentGenerationApi.Instance.RefineMeshyTextToMesh(id);
            }
            else
            {
                var refineStatus =
                    Enum.Parse<RequestStatus>(result.GeneratorResult["refine_status"]?.ToObject<string>(), true);
                if (refineStatus == RequestStatus.Generated)
                {
                    await base.RequestWasGenerated(id, result);
                }
            }
        }

        Task<GameObject> DownloadModel(string url)
        {
            var ret = new TaskCompletionSource<GameObject>();

            IEnumerator DownloadCo()
            {
                var www = UnityWebRequest.Get(url);
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    ret.SetException(new Exception($"{www.error}: {www.downloadHandler?.text}"));
                    yield break;
                }

                var textStream = new MemoryStream(www.downloadHandler.data);
                var loadedObject = new OBJLoader().Load(textStream);
                ret.SetResult(loadedObject);
            }

            Dispatcher.instance.StartCoroutine(DownloadCo());

            return ret.Task;
        }
    }
}
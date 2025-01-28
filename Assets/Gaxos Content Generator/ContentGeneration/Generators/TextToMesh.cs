using System;
using System.Threading.Tasks;
using ContentGeneration.Models;
using ContentGeneration.Models.Meshy;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;

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

        protected override async Task<bool> IsRequestGenerated(Request result)
        {
            if (result.Status == RequestStatus.Generated)
            {
                if (result.GeneratorResult.ContainsKey("refine_status"))
                {
                    if (result.GeneratorResult["refine_status"]!.ToObject<string>() == "GENERATED")
                    {
                        return true;
                    }
                }
                else
                {
                    await ContentGenerationApi.Instance.RefineMeshyTextToMesh(result.ID);
                }
            }

            return false;
        }

        [Serializable]
        class UnityEventUrl : UnityEvent<string>
        {
        }

        [SerializeField] UnityEventUrl _soundUrl;

        protected override void ReportRequestWasJustGenerated(Request request)
        {
            _soundUrl?.Invoke(request.GeneratorResult["refine_result"]!["video_url"]!.ToObject<string>());
                
            var modelUrl = request.GeneratorResult["refine_result"]!["model_urls"]!["obj"]!.ToObject<string>();

            _soundUrl?.Invoke(modelUrl);

            var textures = request.GeneratorResult["refine_result"]!["texture_urls"]!.ToObject<JArray>();

            if (textures != null)
            {
                foreach (var textureDefinition in textures)
                {
                    if (textureDefinition["base_color"] != null)
                    {
                        _soundUrl?.Invoke(textureDefinition["base_color"]!.ToObject<string>());
                        break;
                    }
                }
            }
        }

        protected override Task ReportGeneration(PublishedAsset asset)
        {
            PlayerPrefs.DeleteKey(assetUrlPlayerPrefKey);
            return Task.CompletedTask;
        }
    }
}
using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ContentGeneration;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using Dummiesman;
using Newtonsoft.Json.Linq;
using Sample.Common;
using UnityEngine;
using UnityEngine.Networking;

namespace Sample.Sword
{
    public class SwordContainer : MonoBehaviour
    {
        void Awake()
        {
            ProfileSettings.OnSwordChanged += RefreshImage;
            RefreshImage(ProfileSettings.sword);
        }

        CancellationTokenSource _cancellationTokenSource;

        [SerializeField] GameObject _defaultSword;
        void RefreshImage(PublishedAsset asset)
        {
            _defaultSword.SetActive(true);
            _cancellationTokenSource?.Cancel();
            var thisCancellationToken = _cancellationTokenSource = new CancellationTokenSource();
            if (_prevLoadedObject != null)
            {
                Destroy(_prevLoadedObject);
            }
            RefreshImageAsync(asset, thisCancellationToken.Token).CatchAndLog(this);
        }

        [SerializeField] Vector3 _localPosition;
        [SerializeField] Vector3 _localEulerAngles;
        [SerializeField] Vector3 _localScale = Vector3.one;
        async Task RefreshImageAsync(PublishedAsset asset, CancellationToken token)
        {
            string modelUrl = null; 
            JArray textures = null;
            if(asset != null && asset.Request.GeneratorResult.ContainsKey("model_urls"))
            {
                modelUrl = asset.Request.GeneratorResult["model_urls"]!["obj"]!.ToObject<string>();
                if (asset.Request.GeneratorResult.ContainsKey("refine_result"))
                {
                    modelUrl = asset.Request.GeneratorResult["refine_result"]!["model_urls"]!["obj"]!.ToObject<string>();
                    textures = asset.Request.GeneratorResult["refine_result"]!["texture_urls"]!.ToObject<JArray>();
                }
                else
                {
                    textures = asset.Request.GeneratorResult["texture_urls"]!.ToObject<JArray>();
                }
            }
            if(!string.IsNullOrEmpty(modelUrl))
            {
                var model = await DownloadModel(modelUrl, token);
                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (textures != null)
                {
                    foreach (var textureDefinition in textures)
                    {
                        if (textureDefinition["base_color"] != null)
                        {
                            var texture = await TextureHelper.DownloadImage(textureDefinition["base_color"]!.ToObject<string>(), token);
                            
                            var material = new Material(Shader.Find("Standard"))
                            {
                                mainTexture = texture
                            };
                            var modelRenderer = model.GetComponentInChildren<Renderer>();
                            modelRenderer.material = material;
                            break;
                        }
                    }
                }

                if (token.IsCancellationRequested)
                {
                    return;
                }
                
                _defaultSword.SetActive(false);
                model.transform.SetParent(transform);
                model.transform.localPosition = _localPosition;
                model.transform.localEulerAngles = _localEulerAngles;
                model.transform.localScale = _localScale;
            }
        }

        GameObject _prevLoadedObject;
        Task<GameObject> DownloadModel(string url, CancellationToken cancellationToken)
        {
            var ret = new TaskCompletionSource<GameObject>();
            IEnumerator DownloadCo()
            {
                var www = UnityWebRequest.Get(url);
                yield return www.SendWebRequest();

                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }

                if (www.result != UnityWebRequest.Result.Success)
                {
                    ret.SetException(new Exception($"{www.error}: {www.downloadHandler?.text}"));
                    yield break;
                }

                var textStream = new MemoryStream(www.downloadHandler.data);
                _prevLoadedObject = new OBJLoader().Load(textStream);
                ret.SetResult(_prevLoadedObject);
            }

            Dispatcher.instance.StartCoroutine(DownloadCo());

            return ret.Task;
        }
    }
}
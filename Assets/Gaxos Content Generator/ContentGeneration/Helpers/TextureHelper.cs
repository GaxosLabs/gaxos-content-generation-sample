using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ContentGeneration.Helpers
{
    public static class TextureHelper
    {
        public static Task<Texture2D> DownloadImage(string url)
        {
            return DownloadImage(url, CancellationToken.None);
        }
        public static Task<Texture2D> DownloadImage(string url, CancellationToken cancellationToken)
        {
            var ret = new TaskCompletionSource<Texture2D>();
            IEnumerator DownloadCo()
            {
                var www = UnityWebRequestTexture.GetTexture(url);
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

                var texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                ret.SetResult(texture);
            }

            Dispatcher.instance.StartCoroutine(DownloadCo());

            return ret.Task;
        }
    }
}
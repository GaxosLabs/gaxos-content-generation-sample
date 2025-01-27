using System;
using System.Threading;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Sample.Common
{
    public class PublishedImageRow : MonoBehaviour
    {
        public PublishedAsset publishedAsset { get; private set; }
        public event Action<PublishedAsset> OnPublishedImageChanged;
        CancellationTokenSource _cts;
        public void SetPublishedImage(PublishedAsset publishedAsset)
        {
            this.publishedAsset = publishedAsset;
            gameObject.name = $"Published image - {publishedAsset.ID}";

            _image.enabled = false;
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            _image.DownloadImage(this.publishedAsset.URL, _cts.Token).ContinueInMainThreadWith(t =>
            {
                if(t.IsCanceled)
                    return;
                
                if (t.IsFaulted)
                {
                    Debug.LogException(t.Exception!.InnerException!, this);
                    return;
                }

                _image.enabled = true;
            });

            OnPublishedImageChanged?.Invoke(this.publishedAsset);
        }

        [SerializeField] RawImage _image;
    }
}
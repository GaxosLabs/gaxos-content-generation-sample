using System;
using System.Threading;
using System.Threading.Tasks;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Sample.Base
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RawImage))]
    public abstract class GeneratedImageRawImage : MonoBehaviour
    {
        RawImage _image;

        protected abstract void SubscribeToGeneratedImageChangedEvent(Action<PublishedAsset> changedDelegate);
        protected abstract PublishedAsset GetCurrentGeneratedImage();
        void Awake()
        {
            _image = GetComponent<RawImage>();
            SubscribeToGeneratedImageChangedEvent(RefreshImage);
            RefreshImage(GetCurrentGeneratedImage());
        }

        CancellationTokenSource _cancellationTokenSource;
        [SerializeField] bool _deactivateRendererWhileLoading = true;
        void RefreshImage(PublishedAsset asset)
        {
            _image.texture = null;
            _cancellationTokenSource?.Cancel();
            var thisCancellationToken = _cancellationTokenSource = new CancellationTokenSource();
            if (!string.IsNullOrEmpty(asset?.URL))
            {
                RefreshImageAsync(asset, thisCancellationToken.Token).CatchAndLog(this);
            }
        }

        async Task RefreshImageAsync(PublishedAsset asset, CancellationToken token)
        {
            if (_deactivateRendererWhileLoading)
            {
                _image.enabled = false;
            }
            await _image.DownloadImage(asset.URL, token);
            if(token.IsCancellationRequested)
                return;
            if (_deactivateRendererWhileLoading)
            {
                _image.enabled = true;
            }
        }
    }
}
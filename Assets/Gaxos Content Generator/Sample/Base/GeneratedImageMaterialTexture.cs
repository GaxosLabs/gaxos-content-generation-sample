using System;
using System.Threading;
using System.Threading.Tasks;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using UnityEngine;

namespace Sample.Base
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Renderer))]
    public abstract class GeneratedImageMaterialTexture : MonoBehaviour
    {
        Renderer _renderer;
        
        protected abstract void SubscribeToGeneratedImageChangedEvent(Action<PublishedAsset> changedDelegate);
        protected abstract PublishedAsset GetCurrentGeneratedImage();
        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            SubscribeToGeneratedImageChangedEvent(RefreshImage);
            RefreshImage(GetCurrentGeneratedImage());
        }

        CancellationTokenSource _cancellationTokenSource;
        [SerializeField] bool _deactivateRendererWhileLoading = true;
        [SerializeField] Texture2D _defaultTexture;
        void RefreshImage(PublishedAsset asset)
        {
            _renderer.materials[0].mainTexture = _defaultTexture;
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
                _renderer.enabled = false;
            }
            _renderer.materials[0].mainTexture = await TextureHelper.DownloadImage(asset.URL, token);
            if(token.IsCancellationRequested)
                return;
            if (_deactivateRendererWhileLoading)
            {
                _renderer.enabled = true;
            }
        }
    }
}
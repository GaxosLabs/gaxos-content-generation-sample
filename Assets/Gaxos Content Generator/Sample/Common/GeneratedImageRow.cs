using System;
using System.Threading;
using ContentGeneration;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Sample.Common
{
    public class GeneratedImageRow : MonoBehaviour
    {
        [SerializeField] Toggle _publishToggle;

        void Awake()
        {
            _publishToggle.onValueChanged.AddListener(v =>
            {
                if (v != _generatedAsset.WillBePublic)
                {
                    _publishToggle.interactable = false;
                    _onBusy.Invoke(_generatedAsset, true);
                    ContentGenerationApi.Instance.MakeAssetPublic(_request.ID, _generatedAsset.ID, v).ContinueInMainThreadWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            Debug.LogException(t.Exception!.InnerException!, this);
                        }
                        else
                        {
                            _generatedAsset.WillBePublic = v;
                        }

                        _publishToggle.isOn = _generatedAsset.WillBePublic;
                        _publishToggle.interactable = true;
                        _onBusy.Invoke(_generatedAsset, false);
                    });
                }
            });
        }

        Request _request;
        GeneratedAsset _generatedAsset;
        Action<GeneratedAsset, bool> _onBusy;

        CancellationTokenSource _cts;
        public void SetGeneratedImage(Request request, uint index, 
            GeneratedAsset generatedAsset, Action<GeneratedAsset, bool> onBusy)
        {
            _onBusy = onBusy;
            
            _request = request;
            _generatedAsset = generatedAsset;
            gameObject.name = $"Generated image - {generatedAsset.ID}";

            _image.enabled = false;
            _publishToggle.isOn = generatedAsset.WillBePublic;
            
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            TextureHelper.DownloadImage(_generatedAsset.URL, _cts.Token).ContinueInMainThreadWith(t =>
            {
                if(t.IsCanceled)
                    return;
                
                if (t.IsFaulted)
                {
                    Debug.LogException(t.Exception!.InnerException!, this);
                }
                else
                {
                    _image.texture = t.Result;
                    _image.enabled = true;
                }
            });
        }

        [SerializeField] RawImage _image;

        public bool interactable
        {
            get => _publishToggle.interactable;
            set => _publishToggle.interactable = value;
        }
    }
}
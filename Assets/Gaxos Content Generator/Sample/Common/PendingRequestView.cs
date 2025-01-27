using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContentGeneration;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sample.Common
{
    public class PendingRequestView : MonoBehaviour
    {
        string _requestId;

        public string requestId
        {
            get => _requestId;
            set
            {
                _requestId = value;
                _cancellationTokenSource?.Cancel();
                var cts = _cancellationTokenSource = new CancellationTokenSource();
                RefreshImages(value, cts.Token).CatchAndLog(this); 
            }
        }

        [SerializeField] Transform _generatedImagesContainer;
        [SerializeField] GeneratedImageRow _generatedImageRow;
        [SerializeField] Button _deleteRequestButton;
        [SerializeField] Button _backButton;

        void Awake()
        {
            _deleteRequestButton.onClick.AddListener(DeleteRequest);
        }

        void DeleteRequest()
        {
            _deleteRequestButton.interactable = _backButton.interactable = false;
            _generatedImageRows.ForEach(i => i.interactable = false);
            
            ContentGenerationApi.Instance.DeleteRequest(requestId).ContinueInMainThreadWith(t =>
            {
                _deleteRequestButton.interactable = _backButton.interactable = true;
                _generatedImageRows.ForEach(i => i.interactable = true);
                if (t.IsFaulted)
                {
                    Debug.LogException(t.Exception!.InnerException!, this);
                }
                else
                {
                    _backButton.OnPointerClick(new PointerEventData(EventSystem.current)
                    {
                        button = PointerEventData.InputButton.Left
                    });
                }
            });
        }

        CancellationTokenSource _cancellationTokenSource;
        readonly List<GeneratedImageRow> _generatedImageRows = new();
        async Task RefreshImages(string requestId, CancellationToken cancellationToken)
        {
            foreach (Transform child in _generatedImagesContainer)
            {
                Destroy(child.gameObject);
            }
            _generatedImageRows.Clear();
            
            if (!string.IsNullOrEmpty(requestId))
            {
                var request = await ContentGenerationApi.Instance.GetRequest(requestId);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if(request.Assets!=null)
                {
                    for (uint i = 0; i < request.Assets.Length; i++)
                    {
                        var generatedImage = request.Assets[i];
                        var generatedImageRow = Instantiate(_generatedImageRow, _generatedImagesContainer);
                        generatedImageRow.SetGeneratedImage(request, i, generatedImage, GeneratedImageRowBusy);
                        _generatedImageRows.Add(generatedImageRow);
                    }
                }
            }
        }

        readonly HashSet<string> _busyRows = new();
        void GeneratedImageRowBusy(GeneratedAsset generatedAsset, bool busy)
        {
            if (busy)
            {
                _busyRows.Add(generatedAsset.ID);
            }
            else
            {
                _busyRows.Remove(generatedAsset.ID);
            }

            _deleteRequestButton.interactable =_backButton.interactable = _busyRows.Count == 0;
        }
    }
}
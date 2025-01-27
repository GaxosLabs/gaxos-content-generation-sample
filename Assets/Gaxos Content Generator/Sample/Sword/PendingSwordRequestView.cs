using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using ContentGeneration;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sample.Sword
{
    public class PendingSwordRequestView : MonoBehaviour
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
                Refresh(value, cts.Token).CatchAndLog(this);
            }
        }

        [SerializeField] RawImage _thumbnail;
        [SerializeField] Button _refineButton;
        [SerializeField] GameObject _refineInProgress;
        [SerializeField] GameObject _refineError;
        [SerializeField] Button _deleteRequestButton;
        [SerializeField] Button _backButton;

        void Awake()
        {
            _refineButton.onClick.AddListener(RefineModel);
            _deleteRequestButton.onClick.AddListener(DeleteRequest);
        }

        void RefineModel()
        {
            _deleteRequestButton.interactable =_refineButton.interactable = _backButton.interactable = false;
                
            ContentGenerationApi.Instance.RefineMeshyTextToMesh(requestId).ContinueInMainThreadWith(t =>
            {
                _deleteRequestButton.interactable =_refineButton.interactable = _backButton.interactable = true;
                if (t.IsFaulted)
                {
                    Debug.LogException(t.Exception!.InnerException!, this);
                }
                _cancellationTokenSource?.Cancel();
                var cts = _cancellationTokenSource = new CancellationTokenSource();
                Refresh(requestId, cts.Token).CatchAndLog(this);
            }); 
        }

        void DeleteRequest()
        {
            _deleteRequestButton.interactable =_refineButton.interactable = _backButton.interactable = false;
            
            ContentGenerationApi.Instance.DeleteRequest(requestId).ContinueInMainThreadWith(t =>
            {
                _deleteRequestButton.interactable =_refineButton.interactable = _backButton.interactable = true;
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

        Coroutine _refreshCoroutine;
        async Task Refresh(string requestId, CancellationToken cancellationToken)
        {
            if (_refreshCoroutine != null)
            {
                StopCoroutine(_refreshCoroutine);
            }
            
            if (!string.IsNullOrEmpty(requestId))
            {
                var request = await ContentGenerationApi.Instance.GetRequest(requestId);

                if (request.GeneratorResult == null)
                {
                    _refineButton.gameObject.SetActive(false);
                    _deleteRequestButton.interactable = true;
                
                    _refineInProgress.SetActive(false);
                    _refineError.SetActive(false);
                }
                else if(!request.GeneratorResult.ContainsKey("refine_status"))
                {
                    _refineButton.gameObject.SetActive(true);
                    _deleteRequestButton.interactable = true;
                
                    _refineInProgress.SetActive(false);
                    _refineError.SetActive(false);
                }
                else
                {
                    var refineStatus = Enum.Parse<RequestStatus>(request.GeneratorResult["refine_status"]?.ToObject<string>(), true);
                    if (refineStatus == RequestStatus.Pending)
                    {
                        _refreshCoroutine = StartCoroutine(RefreshCo());

                        IEnumerator RefreshCo()
                        {
                            yield return new WaitForSeconds(3);
                            _refreshCoroutine = null;
                            if (cancellationToken.IsCancellationRequested)
                            {
                                yield break;
                            }
                            Refresh(requestId, cancellationToken).CatchAndLog();
                        }
                    }
                    _refineButton.gameObject.SetActive(false);

                    _deleteRequestButton.interactable = refineStatus != RequestStatus.Pending;
                
                    _refineInProgress.SetActive(refineStatus == RequestStatus.Pending);
                    _refineError.SetActive(refineStatus == RequestStatus.Failed);
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (request.Assets != null)
                {
                    var thumbnail = request.Assets[0];
                    await _thumbnail.DownloadImage(thumbnail.URL, cancellationToken);
                }
            }
        }
    }
}
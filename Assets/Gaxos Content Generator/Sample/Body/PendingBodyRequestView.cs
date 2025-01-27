using System.Threading;
using System.Threading.Tasks;
using ContentGeneration;
using ContentGeneration.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sample.Body
{
    public class PendingBodyRequestView : MonoBehaviour
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
        [SerializeField] Button _deleteRequestButton;
        [SerializeField] Button _backButton;

        void Awake()
        {
            _deleteRequestButton.onClick.AddListener(DeleteRequest);
        }

        void DeleteRequest()
        {
            _deleteRequestButton.interactable = _backButton.interactable = false;
            
            ContentGenerationApi.Instance.DeleteRequest(requestId).ContinueInMainThreadWith(t =>
            {
                _deleteRequestButton.interactable = _backButton.interactable = true;
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

        async Task Refresh(string requestId, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(requestId))
            {
                var request = await ContentGenerationApi.Instance.GetRequest(requestId);

                _deleteRequestButton.interactable = true;

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
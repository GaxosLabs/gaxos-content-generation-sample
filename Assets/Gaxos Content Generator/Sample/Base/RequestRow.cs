using System;
using System.Collections;
using ContentGeneration;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sample.Base
{
    public abstract class RequestRow : MonoBehaviour
    {
        [SerializeField] Button _goToRequestButton;
        [SerializeField] TMP_Text _prompt;
        [SerializeField] TMP_Text _status;

        [SerializeField] Color _colorWhenPending;
        [SerializeField] Color _colorWhenGenerated;
        [SerializeField] Color _colorWhenFailed;

        void Awake()
        {
            _goToRequestButton.onClick.AddListener(() =>
            {
                _onClick.Invoke(request);
            });
        }

        Request request { get; set; }
        Action<Request> _onClick;
        Coroutine _refreshCoroutine;
        public void SetRequest(Request request, Action<Request> onClick)
        {
            this.request = request;
            _onClick = onClick;
            gameObject.name = $"request - {request.ID}";
            _prompt.text = GetPrompt(request);
            _status.text = request.Status.ToString();
            switch (request.Status)
            {
                case RequestStatus.Generated:
                    _status.color = _colorWhenGenerated;
                    break;
                case RequestStatus.Failed:
                    _status.color = _colorWhenFailed;
                    break;
                case RequestStatus.Pending:
                default:
                    _status.color = _colorWhenPending;
                    break;
            }

            _goToRequestButton.interactable = request.Status == RequestStatus.Generated;

            if (request.Status == RequestStatus.Pending && gameObject.activeInHierarchy && _refreshCoroutine == null)
            {
                _refreshCoroutine = StartCoroutine(RefreshStatus());
            }
        }

        protected abstract string GetPrompt(Request request);

        void OnEnable()
        {
            if (_refreshCoroutine != null)
            {
                StopCoroutine(_refreshCoroutine);
            }
            if (request is { Status: RequestStatus.Pending })
            {
                _refreshCoroutine = StartCoroutine(RefreshStatus());
            }
        }

        void OnDestroy()
        {
            if (_refreshCoroutine != null)
            {
                StopCoroutine(_refreshCoroutine);
                _refreshCoroutine = null;
            }
        }

        IEnumerator RefreshStatus()
        {
            if (request.Status == RequestStatus.Pending)
            {
                yield return new WaitForSeconds(5);
                ContentGenerationApi.Instance.GetRequest(request.ID).ContinueInMainThreadWith(t =>
                {
                    _refreshCoroutine = null;
                    if (t.IsFaulted)
                    {
                        Debug.LogException(t.Exception!.InnerException!, this);
                    }
                    else
                    {
                        SetRequest(t.Result, _onClick);
                    }
                });
            }
            _refreshCoroutine = null;
        }
    }
}
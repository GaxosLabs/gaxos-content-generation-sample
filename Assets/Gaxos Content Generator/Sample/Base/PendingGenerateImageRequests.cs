using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentGeneration;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using Sample.Common;
using UnityEngine;
using UnityEngine.Events;
using QueryParameters = ContentGeneration.Models.QueryParameters;

namespace Sample.Base
{
    [DisallowMultipleComponent]
    public abstract class PendingGenerateImageRequests : MonoBehaviour
    {
        protected abstract string subject { get; }

        Coroutine _refreshCoroutine;
        void OnEnable()
        {
            Refresh();
        }

        void OnDisable()
        {
            if (_refreshCoroutine != null)
            {
                StopCoroutine(_refreshCoroutine);
                _refreshCoroutine = null;
            }
        }

        void Refresh()
        {
            RefreshAsync().ContinueInMainThreadWith(t =>
            {
                if (t.IsFaulted)
                {
                    Debug.LogException(t.Exception!.InnerException!, this);
                }

                if (gameObject.activeInHierarchy)
                {
                    _refreshCoroutine = StartCoroutine(RefreshCoroutine());
                }
            });
        }

        [SerializeField] float _refreshIntervalSeconds = 10;
        IEnumerator RefreshCoroutine()
        {
            yield return new WaitForSeconds(_refreshIntervalSeconds);
            _refreshCoroutine = null;
            Refresh();
        }

        [SerializeField] RequestRow _requestRowPrefab;
        readonly Dictionary<string, RequestRow> _rows = new();
        async Task RefreshAsync()
        {
            var requests = await ContentGenerationApi.Instance.GetRequests(
                new QueryParameters
                {
                    FilterByPlayerId = ProfileSettings.playerId,
                    FilterByAssetType = subject,
                    Sort = new []
                    {
                        new QueryParameters.SortBy
                        {
                            Target = QueryParameters.SortTarget.CreatedAt,
                            Direction = QueryParameters.SortDirection.Descending
                        }
                    },
                    Limit = 100,
                    Offset = 0
                });
            var rowsToRemove = _rows.Keys.ToList();
            foreach (var request in requests)
            {
                if (!_rows.ContainsKey(request.ID))
                {
                    var requestRow = Instantiate(_requestRowPrefab, transform);
                    requestRow.SetRequest(request, RequestClicked);
                    _rows.Add(request.ID, requestRow);
                }
                else
                {
                    rowsToRemove.Remove(request.ID);
                }
            }

            foreach (var i in rowsToRemove)
            {
                Destroy(_rows[i].gameObject);
                _rows.Remove(i);
            }
        }

        [Serializable]
        public class RequestUnityEvent : UnityEvent<string>
        {
            
        }

        [SerializeField] RequestUnityEvent _showRequest;
        void RequestClicked(Request obj)
        {
            _showRequest?.Invoke(obj.ID);
        }
    }
}
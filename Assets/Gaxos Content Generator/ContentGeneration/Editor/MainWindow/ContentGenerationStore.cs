using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using Unity.EditorCoroutines.Editor;

namespace ContentGeneration.Editor.MainWindow
{
    public class ContentGenerationStore
    {
        public static ContentGenerationStore Instance = new();

        ContentGenerationStore()
        {
        }

        const uint limit = 1000;
        public QueryParameters.SortBy sortBy = null;
        public const string editorPlayerId = "editor";
        const string filterByAssetType = null;

        public readonly List<Request> Requests = new();
        public event Action<List<Request>> OnRequestsChanged;
        CancellationTokenSource _lastRefreshRequestsListRequest;
        EditorCoroutine _refreshRequestsCoroutine;

        public async Task RefreshRequestsAsync()
        {
            _lastRefreshRequestsListRequest?.Cancel();
            var cts = _lastRefreshRequestsListRequest = new CancellationTokenSource();
            uint currentOffset = 0;
            var requests = new List<Request>();
            OnRequestsChanged?.Invoke(requests);
            while (true)
            {
                var currentRequests = await ContentGenerationApi.Instance.GetRequests(new QueryParameters
                {
                    Limit = limit,
                    Offset = currentOffset,
                    Sort = sortBy == null ? null : new[]
                    {
                        sortBy
                    },
                    FilterByPlayerId = editorPlayerId,
                    FilterByAssetType = filterByAssetType
                });
                if (cts.IsCancellationRequested)
                {
                    return;
                }

                requests.AddRange(currentRequests);
                OnRequestsChanged?.Invoke(requests);
                if (currentRequests.Length < limit)
                {
                    break;
                }

                currentOffset += limit;
            }
            Requests.Clear();
            Requests.AddRange(requests);
            OnRequestsChanged?.Invoke(Requests);

            if (Requests.Any(i => i.Status == RequestStatus.Pending))
            {
                if (_refreshRequestsCoroutine == null)
                {
                    IEnumerator RefreshRequestListCo()
                    {
                        yield return new EditorWaitForSeconds(3);
                        RefreshRequestsAsync().CatchAndLog();
                        _refreshRequestsCoroutine = null;
                    }

                    _refreshRequestsCoroutine = EditorCoroutineUtility.StartCoroutineOwnerless(RefreshRequestListCo());
                }
            }
            else
            {
                if (_refreshRequestsCoroutine != null)
                {
                    EditorCoroutineUtility.StopCoroutine(_refreshRequestsCoroutine);
                    _refreshRequestsCoroutine = null;
                }
            }
        }

        public Stats stats { get; private set; }
        public event Action<Stats> OnStatsChanged;
        CancellationTokenSource _lastRefreshStatsRequest;

        public async Task RefreshStatsAsync()
        {
            _lastRefreshStatsRequest?.Cancel();
            var cts = _lastRefreshRequestsListRequest = new CancellationTokenSource();
            var currentStats = await ContentGenerationApi.Instance.GetStats();
            if (cts.IsCancellationRequested)
            {
                return;
            }

            stats = currentStats;
            OnStatsChanged?.Invoke(currentStats);
        }
        
        public readonly List<Favorite> Favorites = new();
        public event Action<List<Favorite>> OnFavoritesChanged;
        CancellationTokenSource _lastRefreshFavoritesListRequest;

        public async Task RefreshFavoritesAsync()
        {
            _lastRefreshFavoritesListRequest?.Cancel();
            var cts = _lastRefreshFavoritesListRequest = new CancellationTokenSource();

            var currentFavorites = await ContentGenerationApi.Instance.GetFavorites();
            if (cts.IsCancellationRequested)
            {
                return;
            }

            Favorites.Clear();
            Favorites.AddRange(currentFavorites);
            OnFavoritesChanged?.Invoke(Favorites);
        }
    }
}
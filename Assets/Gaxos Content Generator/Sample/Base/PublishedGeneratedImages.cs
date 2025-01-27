using System.Threading;
using System.Threading.Tasks;
using ContentGeneration;
using ContentGeneration.Helpers;
using Sample.Common;
using UnityEngine;

namespace Sample.Base
{
    [DisallowMultipleComponent]
    public abstract class PublishedGeneratedImages : MonoBehaviour
    {
        protected abstract string subject { get; }

        CancellationTokenSource _cancellationTokenSource;
        void OnEnable()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            RefreshAsync(_cancellationTokenSource.Token).CatchAndLog(this);
        }

        [SerializeField] PublishedImageRow _publishedImageRow;
        async Task RefreshAsync(CancellationToken token)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            var publishedImages = await ContentGenerationApi.Instance.GetPublishedAssets(
                new ContentGeneration.Models.QueryParameters
                {
                    FilterByPlayerId = ProfileSettings.playerId,
                    FilterByAssetType = subject,
                    Sort = new []
                    {
                        new ContentGeneration.Models.QueryParameters.SortBy
                        {
                            Target = ContentGeneration.Models.QueryParameters.SortTarget.CreatedAt,
                            Direction = ContentGeneration.Models.QueryParameters.SortDirection.Descending
                        }
                    }
                });

            if (token.IsCancellationRequested)
            {
                return;
            }
            
            foreach (var publishedImage in publishedImages)
            {
                var publishedImageRow = Instantiate(_publishedImageRow, transform);
                publishedImageRow.SetPublishedImage(publishedImage);
            }
        }
    }
}
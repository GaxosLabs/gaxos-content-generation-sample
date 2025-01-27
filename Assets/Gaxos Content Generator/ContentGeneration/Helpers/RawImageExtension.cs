using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace ContentGeneration.Helpers
{
    public static class RawImageExtension
    {
        public static Task DownloadImage(this RawImage me, string url)
        {
            return me.DownloadImage(url, CancellationToken.None);
        }
        public static async Task DownloadImage(this RawImage me, string url, CancellationToken cancellationToken)
        {
            var texture = await TextureHelper.DownloadImage(url, cancellationToken);

            if(cancellationToken.IsCancellationRequested)
                return;
        
            me.texture = texture;
            var aspectRatioFitter = me.GetComponent<AspectRatioFitter>();
            if (aspectRatioFitter != null)
            {
                aspectRatioFitter.aspectRatio = me.texture.width / (float)me.texture.height;
            }
        }
    }
}
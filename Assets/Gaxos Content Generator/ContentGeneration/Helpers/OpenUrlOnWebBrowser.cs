using UnityEngine;

namespace ContentGeneration.Helpers
{
    public class OpenUrlOnWebBrowser : MonoBehaviour
    {
        public void UrlReceived(string url)
        {
            Application.OpenURL(url);
        }
    }
}
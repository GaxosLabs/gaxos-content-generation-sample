using UnityEngine;

namespace Sample.Common
{
    public class HideOnStart : MonoBehaviour
    {
        void Start()
        {
            gameObject.SetActive(false);
        }
    }
}
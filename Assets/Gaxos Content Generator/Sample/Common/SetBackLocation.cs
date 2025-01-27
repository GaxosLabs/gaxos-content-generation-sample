using UnityEngine;
using UnityEngine.UI;

namespace Sample.Common
{
    [RequireComponent(typeof(Button))]
    public class SetBackLocation : MonoBehaviour
    {
        public GameObject location { get; set; }
        void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                location.SetActive(true);
            });
        }
    }
}
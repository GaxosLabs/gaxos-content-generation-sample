using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sample.Common
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public class RestartButton : MonoBehaviour
    {
        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                ProfileSettings.playerId = Guid.NewGuid().ToString();
                ProfileSettings.shield = null;
                ProfileSettings.flag = null;
                ProfileSettings.sword = null;
                ProfileSettings.body = null;
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            });
        }
    }
}
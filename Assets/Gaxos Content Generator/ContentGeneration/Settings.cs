using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ContentGeneration
{
    [DefaultExecutionOrder(-10000)]
    public class Settings : ScriptableObject
    {
#if UNITY_EDITOR
        const string Path = "Assets/" + nameof(ContentGeneration) + "." + nameof(Settings) + ".asset";
        [InitializeOnLoadMethod]
        static void CheckCreated()
        {
            var settings = AssetDatabase.LoadAssetAtPath<Settings>(Path);
            if (settings == null)
            {
                settings = CreateInstance<Settings>();
                AssetDatabase.CreateAsset(settings, Path);
                AssetDatabase.SaveAssets();
            }

            var preloadedAssets = PlayerSettings.GetPreloadedAssets();
            if(!preloadedAssets.Contains(settings))
            {
                PlayerSettings.SetPreloadedAssets(preloadedAssets.Append(settings).ToArray());
            }
        }
#endif
        
        public static Settings instance { get; private set; }
        void OnEnable()
        {
            instance = this;
        }

        [SerializeField]
        public string apiKey;
    }
}
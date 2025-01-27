using System.Collections.Generic;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using UnityEditor;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components
{
    public class Configuration : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<Configuration, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        public Configuration()
        {
            var apiKey = this.Q<TextField>("apiKey");
            apiKey.value = Settings.instance.apiKey;
            apiKey.RegisterValueChangedCallback(v =>
            {
                Settings.instance.apiKey = v.newValue;
                EditorUtility.SetDirty(Settings.instance);
                AssetDatabase.SaveAssets();
            });
            
            var credits = this.Q<TextField>("credits");
            var storage = this.Q<TextField>("storage");
            var requests = this.Q<TextField>("requests");
            var refresh = this.Q<Button>("refresh");
            void Refresh(Stats v)
            {
                if (v == null)
                {
                    credits.value = storage.value = requests.value = "";
                    return;
                }
                credits.value =  v.Credits.ToString();
                storage.value =  v.Storage.ToString();
                requests.value =  v.Requests.ToString();
            }
            ContentGenerationStore.Instance.OnStatsChanged += Refresh;
            Refresh(ContentGenerationStore.Instance.stats);
            refresh.clicked += () =>
            {
                if (!refresh.enabledSelf)
                    return;
                refresh.SetEnabled(false);
                ContentGenerationStore.Instance.RefreshStatsAsync().Finally(() =>
                {
                    refresh.SetEnabled(true);
                });
            };
            if(!string.IsNullOrEmpty(Settings.instance.apiKey))
            {
                ContentGenerationStore.Instance.RefreshStatsAsync().CatchAndLog();
            }
        }
    }
}
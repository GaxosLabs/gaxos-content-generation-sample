using System.Threading.Tasks;
using ContentGeneration.Editor.MainWindow;
using ContentGeneration.Helpers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor
{
    [CustomEditor(typeof(Settings))]
    public class SettingsInspector : UnityEditor.Editor
    {
        [SerializeField] VisualTreeAsset _rootAsset;
        Button _refreshButton;
        TextField _credits;
        TextField _storage;
        TextField _requests;

        public override VisualElement CreateInspectorGUI()
        {
            var rootVisualElement = _rootAsset.Instantiate();

            _credits = rootVisualElement.Q<TextField>("credits");
            _credits.value = "";
            _storage = rootVisualElement.Q<TextField>("storage");
            _storage.value = "";
            _requests = rootVisualElement.Q<TextField>("requests");
            _requests.value = "";
            _refreshButton = rootVisualElement.Q<Button>("refresh");
            _refreshButton.RegisterCallback<ClickEvent>(_ =>
            {
                Refresh();
            });

            Refresh();
            return rootVisualElement;
        }

        void Refresh()
        {
            if(!_refreshButton.enabledSelf)
                return;
                
            _refreshButton.SetEnabled(false);
            RefreshAsync().ContinueInMainThreadWith(t =>
            {
                if (t.IsFaulted)
                {
                    Debug.LogException(t.Exception);
                }
                    
                _refreshButton.SetEnabled(true);
            });

        }

        async Task RefreshAsync()
        {
            _credits.value = "";
            _storage.value = "";
            _requests.value = "";
            await ContentGenerationStore.Instance.RefreshStatsAsync();
            _credits.value =  ContentGenerationStore.Instance.stats.Credits.ToString();
            _storage.value =  ContentGenerationStore.Instance.stats.Storage.ToString();
            _requests.value = ContentGenerationStore.Instance.stats.Requests.ToString();
        }
    }
}

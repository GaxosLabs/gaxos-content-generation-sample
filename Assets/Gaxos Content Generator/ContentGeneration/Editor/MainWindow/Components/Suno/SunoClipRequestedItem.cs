using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ContentGeneration.Editor.MainWindow.Components.Meshy;
using ContentGeneration.Editor.MainWindow.Components.RequestsList;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Suno
{
    public class SunoClipRequestedItem : VisualElementComponent, IRequestedItem
    {
        public new class UxmlFactory : UxmlFactory<SunoClipRequestedItem, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        Button previewButton => this.Q<Button>("previewButton");
        Button saveButton => this.Q<Button>("saveButton");

        RequestedItemCommon requestedItemCommon => this.Q<RequestedItemCommon>();

        public SunoClipRequestedItem()
        {
            requestedItemCommon.OnDeleted += () =>
            {
                OnDeleted?.Invoke();
            };
            requestedItemCommon.OnRefreshed += v => value = v;

            previewButton.SetEnabled(false);
            previewButton.clicked += () =>
            {
                Application.OpenURL(value.GeneratorResult["audio_url"]!.ToObject<string>());
            };
            saveButton.SetEnabled(false);
            saveButton.clicked += () =>
            {
                if (!saveButton.enabledSelf)
                    return;

                saveButton.SetEnabled(false);
                Save(value).ContinueInMainThreadWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        Debug.LogException(t.Exception!.InnerException);
                    }

                    saveButton.SetEnabled(true);
                });
            };
        }

        public event Action OnDeleted;

        public Request value
        {
            get => requestedItemCommon.value;
            set
            {
                requestedItemCommon.value = value;

                previewButton.SetEnabled(value?.GeneratorResult != null);
                saveButton.SetEnabled(value?.GeneratorResult != null);
            }
        }

        public async Task Save(Request request)
        {
            var path = EditorUtility.SaveFilePanel(
                "Save clip location",
                "Assets/",
                "", "mp3");

            if (path.Length == 0) return;

            var model = await MeshyModelHelper.DownloadFileAsync(request.GeneratorResult["audio_url"]!.ToObject<string>());
            await File.WriteAllBytesAsync(path, model);

            if (path.StartsWith(Application.dataPath))
            {
                AssetDatabase.Refresh();
            }
        }
    }
}
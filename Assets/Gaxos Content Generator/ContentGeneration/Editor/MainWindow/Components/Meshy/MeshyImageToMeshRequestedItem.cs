using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ContentGeneration.Editor.MainWindow.Components.RequestsList;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Meshy
{
    public class MeshyImageToMeshRequestedItem : VisualElementComponent, IRequestedItem
    {
        public new class UxmlFactory : UxmlFactory<MeshyImageToMeshRequestedItem, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        Button saveButton => this.Q<Button>("saveButton");

        RequestedItemCommon requestedItemCommon => this.Q<RequestedItemCommon>();

        public MeshyImageToMeshRequestedItem()
        {
            requestedItemCommon.OnDeleted += () => { OnDeleted?.Invoke(); };
            requestedItemCommon.OnRefreshed += v => value = v;

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

                if (value == null)
                    return;

                saveButton.SetEnabled(value.GeneratorResult != null);
            }
        }

        public async Task Save(Request request)
        {
            var path = EditorUtility.SaveFilePanel(
                "Save model location",
                "Assets/",
                "", "glb");

            if (path.Length == 0) return;

            var model = await MeshyModelHelper.DownloadFileAsync(
                request.GeneratorResult["model_urls"]!["glb"]!.ToObject<string>());
            await File.WriteAllBytesAsync(path, model);

            if (path.StartsWith(Application.dataPath))
            {
                AssetDatabase.Refresh();
            }
        }
    }
}
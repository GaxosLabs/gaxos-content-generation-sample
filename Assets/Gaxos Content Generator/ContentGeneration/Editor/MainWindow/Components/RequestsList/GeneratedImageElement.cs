using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ContentGeneration.Models;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.RequestsList
{
    public class GeneratedImageElement : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<GeneratedImageElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        Image image => this.Q<Image>("image");
        Button saveToProject => this.Q<Button>("saveToProject");

        public GeneratedImageElement() : this(null)
        {
        }

        public static void SaveImageToProject(params Texture2D[] images)
        {
            var path = EditorUtility.SaveFilePanel(
                "Save texture as PNG",
                "Assets/",
                "",
                "png");

            if (path.Length == 0) return;
                
            if(images.Length == 1)
            {
                var pngData = images[0].EncodeToPNG();
                if (pngData != null)
                {
                    File.WriteAllBytes(path, pngData);
                }
            }
            else
            {
                var filename = Path.GetFileNameWithoutExtension(path);
                var extension = Path.GetExtension(path);
                path = Path.GetDirectoryName(path);

                for (var i = 0; i < images.Length; i++)
                {
                    var pngData = images[i].EncodeToPNG();
                    File.WriteAllBytes(Path.Combine(path, $"{filename}.{i+1}{extension}"), pngData);
                }
            }
            AssetDatabase.Refresh();
        }

        public readonly GeneratedAsset generatedAsset;
        public GeneratedImageElement(GeneratedAsset v)
        {
            generatedAsset = v;
            image.image = null;
            image.AddManipulator(new Clickable(_ =>
            {
                Application.OpenURL(generatedAsset.URL);
            }));
            saveToProject.SetEnabled(false);
            EditorCoroutineUtility.StartCoroutine(
                LoadImage(generatedAsset.URL), this);
            
            saveToProject.RegisterCallback<ClickEvent>(_ =>
            {
                if (!saveToProject.enabledSelf || image.image == null) return;
                
                SaveImageToProject((Texture2D)image.image);
            });
        }

        IEnumerator LoadImage(string imageUrl)
        {
            var www = UnityWebRequestTexture.GetTexture(imageUrl);
            www.SetRequestHeader("Authorization", $"Bearer {Settings.instance.apiKey}");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                throw new Exception($"{www.error}: {www.downloadHandler?.text}");
            }

            var texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            image.image = texture;
            saveToProject.SetEnabled(true);
        }

        public void Refresh()
        {
            EditorCoroutineUtility.StartCoroutine(
                LoadImage(generatedAsset.URL), this);
        }
    }
}
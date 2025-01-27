using System.IO;
using UnityEditor;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components
{
    public abstract class VisualElementComponent : VisualElement
    {
        static string _componentsBasePath;
        protected static string componentsBasePath
        {
            get
            {
                if (_componentsBasePath == null)
                {
                    var settingsMonoScript = MonoScript.FromScriptableObject(Settings.instance);
                    _componentsBasePath = 
                        Path.Combine(Path.GetDirectoryName(AssetDatabase.GetAssetPath(settingsMonoScript))!, 
                            "Editor");
                }
                return _componentsBasePath;
            }
        }

        protected VisualElementComponent()
        {
            var fullTypeName = GetType().FullName!.Replace("ContentGeneration.Editor.", "");
            var assetPath = $"{componentsBasePath}/{fullTypeName.Replace('.', Path.DirectorySeparatorChar)}.uxml";
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath);
            asset.CloneTree(this);
        }
    }
}
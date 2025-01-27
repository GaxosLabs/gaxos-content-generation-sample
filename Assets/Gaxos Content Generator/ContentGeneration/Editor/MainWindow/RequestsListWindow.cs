using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow
{
    public class RequestsListWindow : EditorWindow
    {
        [SerializeField] VisualTreeAsset _root;

        [MenuItem("AI Content Generation/Requests list")]
        public static void ShowRequestsListWindow()
        {
            var wnd = GetWindow<RequestsListWindow>();
            wnd.minSize = new Vector2(500, 300);
            wnd.titleContent = new GUIContent("AI Generation Requests");
        }

        public void CreateGUI()
        {
            var rootInstance = _root.Instantiate();
            rootInstance.style.height = new StyleLength(new Length(100, LengthUnit.Percent));

            rootVisualElement.Add(rootInstance);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components
{
    public class TabsListContainer : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<TabsListContainer, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield return new UxmlChildElementDescription(typeof(Tab)); }
            }
        }

        DropdownField list => this.Q<DropdownField>("list");
        public override VisualElement contentContainer => this.Q<VisualElement>("contentContainer");

        readonly HashSet<string> createdTabs = new();

        public TabsListContainer()
        {
            list.RegisterValueChangedCallback(v =>
            {
                foreach (var visualElement in contentContainer!.Children())
                {
                    if (visualElement is Tab t)
                    {
                        if (t.tabName == v.newValue)
                        {
                            visualElement.style.display = DisplayStyle.Flex;
                            continue;
                        }
                    }
                    visualElement.style.display = DisplayStyle.None;
                }
            });
            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                var selectIndex = 0;
                var children = contentContainer!.Children().ToArray();
                for (int i = 0; i < children.Length; i++)
                {
                    var visualElement = children[i];
                    if (visualElement is Tab t)
                    {
                        if (createdTabs.Add(t.tabName))
                        {
                            list.choices.Add(t.tabName);
                        }
                        if (MainWindow.instance.ShowFavorite != null)
                        {
                            var generatorVisualElements = t.Children().
                                Where(i => i is IGeneratorVisualElement).Cast<IGeneratorVisualElement>();
                            foreach (var generatorVisualElement in generatorVisualElements)
                            {
                                if (generatorVisualElement.generator == MainWindow.instance.ShowFavorite?.Generator)
                                {
                                    selectIndex = i;
                                    generatorVisualElement.Show(MainWindow.instance.ShowFavorite);
                                    MainWindow.instance.ShowFavorite = null;
                                }
                            }
                        }
                    }
                }

                list.index = selectIndex;
            });
        }
    }
}
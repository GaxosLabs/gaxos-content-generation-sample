using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components
{
    public class TabsContainer : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<TabsContainer, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield return new UxmlChildElementDescription(typeof(Tab)); }
            }
        }

        public override VisualElement contentContainer => this.Q<VisualElement>("contentContainer");
        VisualElement tabToggles => this.Q<VisualElement>("tabToggles");

        readonly Dictionary<string, RadioButton> createdTabs = new();

        public TabsContainer()
        {
            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                RadioButton showRadioButton = null;
                foreach (var visualElement in contentContainer!.Children())
                {
                    if (visualElement is Tab t)
                    {
                        if (!createdTabs.ContainsKey(t.tabName))
                        {
                            var tabToggle = new RadioButton(t.tabName);
                            if (showRadioButton == null)
                            {
                                showRadioButton = tabToggle;
                            }
                            createdTabs.Add(t.tabName, tabToggle);
                            tabToggle.RegisterValueChangedCallback(v =>
                            {
                                visualElement.style.display = v.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                            });
                            tabToggles.Add(tabToggle);
                            visualElement.style.display = DisplayStyle.None;
                        }
                        if (MainWindow.instance.ShowFavorite != null)
                        {
                            var generatorVisualElements = t.Children().
                                Where(i => i is IGeneratorVisualElement).Cast<IGeneratorVisualElement>();
                            foreach (var generatorVisualElement in generatorVisualElements)
                            {
                                if (generatorVisualElement.generator == MainWindow.instance.ShowFavorite?.Generator)
                                {
                                    showRadioButton = createdTabs[t.tabName];
                                    generatorVisualElement.Show(MainWindow.instance.ShowFavorite);
                                    MainWindow.instance.ShowFavorite = null;
                                }
                            }
                        }
                    }
                }

                if (showRadioButton != null)
                {
                    foreach (var radioButton in createdTabs.Values)
                    {
                        radioButton.value = showRadioButton == radioButton;
                    }
                }
            });
        }
    }
}
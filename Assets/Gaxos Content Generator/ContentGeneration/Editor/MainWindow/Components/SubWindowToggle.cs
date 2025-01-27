using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components
{
    public class SubWindowToggle : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<SubWindowToggle, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription _subWindowName = new() { name = "Sub-Window-Name" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var element = (SubWindowToggle)ve;
                element.subWindowName = _subWindowName.GetValueFromBag(bag, cc);
            }
        }

        public event Action<SubWindowToggle, bool> OnToggled;
        public SubWindowToggle()
        {
            toggle.RegisterValueChangedCallback(v =>
            {
                OnToggled?.Invoke(this, v.newValue);
            });
        }

        public void ToggleOff()
        {
            toggle.value = false;
        }

        public void ToggleOn()
        {
            toggle.value = true;
        }


        Label label => this.Q<Label>("label");
        Toggle toggle => this.Q<Toggle>("subWindowToggle");
        protected VisualElement icon => this.Q<VisualElement>("icon");

        public virtual string subWindowName
        {
            get => label.text;
            set
            {
                label.text = value;
                icon.style.backgroundImage = new StyleBackground(
                    AssetDatabase.LoadAssetAtPath<Sprite>(
                        System.IO.Path.Combine(componentsBasePath, $"MainWindow/{value}.png")));
            }
        }
    }
}
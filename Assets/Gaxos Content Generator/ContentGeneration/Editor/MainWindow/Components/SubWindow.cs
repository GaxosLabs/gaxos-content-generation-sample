using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components
{
    public class SubWindow : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<SubWindow, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription _subWindowName = new() { name = "Sub-Window-Name" };
            readonly UxmlStringAttributeDescription _subWindowIcon = new() { name = "Sub-Window-Icon" };
            readonly UxmlBoolAttributeDescription _foldable = new() { name = "Foldable" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get
                {
                    yield return new UxmlChildElementDescription(typeof (VisualElement));
                }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var element = (SubWindow)ve;
                element.subWindowName = _subWindowName.GetValueFromBag(bag, cc);
                element.subWindowIcon = _subWindowIcon.GetValueFromBag(bag, cc);
                element.foldable = _foldable.GetValueFromBag(bag, cc);
            }
        }
        
        Label label => this.Q<Label>("label");
        VisualElement icon => this.Q<VisualElement>("icon");
        VisualElement contents => this.Q<VisualElement>("contents");
        Button foldButton => this.Q<Button>("foldButton");

        public string subWindowName
        {
            get => label.text;
            set
            {
                label.text = value;
                SetIcon();
            }
        }
        
        string _subWindowIcon;
        public string subWindowIcon
        {
            get => _subWindowIcon;
            set
            {
                _subWindowIcon = value;
                SetIcon();
            }
        }
        
        void SetIcon()
        {
            icon.style.backgroundImage = new StyleBackground(
                AssetDatabase.LoadAssetAtPath<Sprite>(
                    System.IO.Path.Combine(componentsBasePath, $"MainWindow/{(string.IsNullOrEmpty(subWindowIcon) ? subWindowName : subWindowIcon)}.png")));
        }
        
        bool _foldable;
        public bool foldable
        {
            get => _foldable;
            set
            {
                _foldable = value;
                foldButton.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
                if (!foldable)
                {
                    folded = false;
                }
                else
                {
                    folded = true;
                }
            }
        }
        
        bool _folded;
        public bool folded
        {
            get => _folded;
            set
            {
                _folded = value;
                
                contents.style.display = value ? DisplayStyle.None : DisplayStyle.Flex;

                foldButton.RemoveFromClassList("folded");
                if (value)
                {
                    foldButton.AddToClassList("folded");
                }
            }
        }

        public override VisualElement contentContainer => contents;

        public SubWindow()
        {
            foldButton.clicked += () =>
            {
                folded = !folded;
            };
        }
    }
}

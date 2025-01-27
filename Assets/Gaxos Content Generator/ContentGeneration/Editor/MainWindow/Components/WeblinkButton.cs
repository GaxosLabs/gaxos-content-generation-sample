using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components
{
    public class WeblinkButton  : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<WeblinkButton, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription _title = new() { name = "Title" };
            readonly UxmlStringAttributeDescription _url = new() { name = "URl" };
            readonly UxmlStringAttributeDescription _icon = new() { name = "Icon" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var element = (WeblinkButton)ve;
                element.title = _title.GetValueFromBag(bag, cc);
                element.URL = _url.GetValueFromBag(bag, cc);
                element.sprite = _icon.GetValueFromBag(bag, cc);
            }
        }

        public WeblinkButton()
        {
            button.clicked += () =>
            {
                Application.OpenURL(URL);
            };
        }

        Button button => this.Q<Button>("weblinkButton");
        Label label => this.Q<Label>("label");
        VisualElement icon => this.Q<VisualElement>("icon");

        public string title
        {
            get => label.text;
            set
            {
                label.text = value;
            }
        }
        public string URL;
        string _sprite;
        public string sprite
        {
            get => _sprite;
            set
            {
                _sprite = value;
                icon.style.backgroundImage = new StyleBackground(
                    AssetDatabase.LoadAssetAtPath<Sprite>(
                        System.IO.Path.Combine(componentsBasePath, $"MainWindow/{value}.png")));
            }
        }
    }
}
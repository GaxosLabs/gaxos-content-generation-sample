using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components
{
    public class ImageSelection : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<ImageSelection, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription _label = new() { name = "Label" };
            readonly UxmlStringAttributeDescription _buttonText = new() { name = "Button-Text" };
            
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var element = (ImageSelection)ve;
                element.label = _label.GetValueFromBag(bag, cc);
                element.buttonText = _buttonText.GetValueFromBag(bag, cc);
            }
        }
        
        Label labelElement => this.Q<Label>("labelElement");
        public string label
        {
            get => labelElement.text;
            set => labelElement.text = value;
        }

        Button selectImage => this.Q<Button>("selectImage");
        public string buttonText
        {
            get => selectImage.text;
            set => selectImage.text = value;
        }
        
        Image imageElement => this.Q<Image>("imageElement");
        public Texture image
        {
            get => imageElement.image;
            set => imageElement.image = value;
        }
        
        public ImageSelection()
        {
            selectImage.RegisterCallback<ClickEvent>(_ =>
            {
                var path = EditorUtility.OpenFilePanelWithFilters(
                    "Select image",
                    "Assets/",
                    new[]
                    {
                        "Image (PNG)", "png"
                    });

                if (path.Length == 0) return;

                var bytes = File.ReadAllBytes(path);
                var texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);
                texture.Apply();
                image = texture;
                OnChanged?.Invoke();
            });
        }

        public event Action OnChanged;
    }
}
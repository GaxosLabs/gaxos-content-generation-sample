using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components
{
    public class PromptInput : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<PromptInput, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            readonly UxmlStringAttributeDescription _placeholder = new() { name = "Placeholder" };
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var element = (PromptInput)ve;
                element.value = _placeholder.GetValueFromBag(bag, cc);
            }
        }

        TextField text => this.Q<TextField>("text");
        public string value
        {
            get => text.value;
            set => text.value = value;
        }

        public event Action<string> OnChanged;
        public PromptInput()
        {
            text.RegisterValueChangedCallback(v => OnChanged?.Invoke(v.newValue));
        }
    }
}

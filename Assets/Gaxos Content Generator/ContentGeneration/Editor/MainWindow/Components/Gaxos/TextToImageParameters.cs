using System;
using System.Collections.Generic;
using ContentGeneration.Models;
using ContentGeneration.Models.Gaxos;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Gaxos
{
    public class TextToImageParameters : VisualElementComponent, IParameters<GaxosTextToImageParameters>
    {
        public new class UxmlFactory : UxmlFactory<TextToImageParameters, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            readonly UxmlBoolAttributeDescription _hidePrompt = new() { name = "HidePrompt" };
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var element = (TextToImageParameters)ve;
                element.hidePrompt = _hidePrompt.GetValueFromBag(bag, cc);
            }
        }

        SliderInt width => this.Q<SliderInt>("width");
        SliderInt height => this.Q<SliderInt>("height");

        GaxosParametersElement gaxosParametersElement => this.Q<GaxosParametersElement>("gaxosParametersElement");
        public GenerationOptionsElement generationOptions => this.Q<GenerationOptionsElement>("generationOptions");

        public bool hidePrompt
        {
            get => gaxosParametersElement.hidePrompt;
            set => gaxosParametersElement.hidePrompt = value;
        }

        public TextToImageParameters()
        {
            gaxosParametersElement.OnCodeHasChanged = CodeHasChanged;
            width.RegisterValueChangedCallback(_ => CodeHasChanged());
            height.RegisterValueChangedCallback(_ => CodeHasChanged());
            CodeHasChanged();
        }

        public Action codeHasChanged { get; set; }

        void CodeHasChanged()
        {
            codeHasChanged?.Invoke();
        }

        public bool Valid(bool updateUI)
        {
            if (!gaxosParametersElement.Valid(updateUI))
            {
                return false;
            }

            return true;
        }

        public void ApplyParameters(GaxosTextToImageParameters gaxosParameters)
        {
            gaxosParameters.Width = (uint)width.value;
            gaxosParameters.Height = (uint)height.value;
            gaxosParametersElement.ApplyParameters(gaxosParameters);
        }

        public string GetCode()
        {
            return gaxosParametersElement.GetCode() +
                   $"\t\tWidth = {width.value}\n" +
                   $"\t\tHeight = {height.value}\n";
        }

        public void Show(Favorite favorite)
        {
            var gaxosTextToImageParameters = favorite.GeneratorParameters.ToObject<GaxosTextToImageParameters>();
            gaxosParametersElement.Show(gaxosTextToImageParameters);
            generationOptions.Show(favorite.GenerationOptions);
            
            width.value = (int)gaxosTextToImageParameters.Width;
            height.value = (int)gaxosTextToImageParameters.Height;
            CodeHasChanged();
        }
    }
}
using System;
using System.Collections.Generic;
using ContentGeneration.Models;
using ContentGeneration.Models.DallE;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.DallE
{
    public class TextToImageParameters : VisualElementComponent, IParameters<DallETextToImageParameters>
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

        DallEParametersElement dallEParametersElement => this.Q<DallEParametersElement>("dallEParametersElement");
        public GenerationOptionsElement generationOptions => this.Q<GenerationOptionsElement>("generationOptions");

        public bool hidePrompt
        {
            get => dallEParametersElement.hidePrompt;
            set => dallEParametersElement.hidePrompt = value;
        }
        
        public TextToImageParameters()
        {
            dallEParametersElement.OnCodeChanged += CodeHasChanged;
            CodeHasChanged();
        }

        public Action codeHasChanged { get; set; }
        void CodeHasChanged()
        {
            codeHasChanged?.Invoke();
        }

        public bool Valid(bool updateUI)
        {
            if (!dallEParametersElement.Valid(updateUI))
            {
                return false;
            }

            return true;
        }

        public void ApplyParameters(DallETextToImageParameters dallEParameters)
        {
            dallEParametersElement.ApplyParameters(dallEParameters);
        }

        public string GetCode()
        {
            return dallEParametersElement?.GetCode();
        }

        public void Show(Favorite favorite)
        {
            var dallEParameters = favorite.GeneratorParameters.ToObject<DallETextToImageParameters>();
            dallEParametersElement.Show(dallEParameters);
            generationOptions.Show(favorite.GenerationOptions);
            
            CodeHasChanged();
        }
    }
}
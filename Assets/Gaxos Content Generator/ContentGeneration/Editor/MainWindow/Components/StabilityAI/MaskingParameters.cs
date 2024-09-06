using System;
using System.Collections.Generic;
using ContentGeneration.Models.Stability;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.StabilityAI
{
    public class MaskingParameters : VisualElementComponent, IParameters<StabilityMaskedImageParameters>
    {
        public new class UxmlFactory : UxmlFactory<MaskingParameters, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }
        
        DropdownField engine => this.Q<DropdownField>("engine");
        ImageSelection image => this.Q<ImageSelection>("image");
        Label imageRequired => this.Q<Label>("imageRequired");
        ImageSelection mask => this.Q<ImageSelection>("mask");
        Label maskRequired => this.Q<Label>("maskRequired");
        EnumField maskSource => this.Q<EnumField>("maskSource");
        StabilityParametersElement stabilityParameters => this.Q<StabilityParametersElement>("stabilityParameters");
        public GenerationOptionsElement generationOptions => this.Q<GenerationOptionsElement>("generationOptions");

        public MaskingParameters()
        {
            stabilityParameters.OnCodeHasChanged = CodeHasChanged;
            
            var engines = new[]
            {
                "esrgan-v1-x2plus",
                "stable-diffusion-xl-1024-v0-9",
                "stable-diffusion-xl-1024-v1-0",
                "stable-diffusion-v1-6",
                "stable-diffusion-512-v2-1",
                "stable-diffusion-xl-beta-v2-2-2",
            };
            engine.choices = new List<string>(engines);
            engine.index = Array.IndexOf(engines, "stable-diffusion-v1-6");

            imageRequired.style.visibility = Visibility.Hidden;
            maskRequired.style.visibility = Visibility.Hidden;

            mask.style.display = (MaskSource)maskSource.value == MaskSource.InitImageAlpha
                ? DisplayStyle.None
                : DisplayStyle.Flex;
            maskSource.RegisterValueChangedCallback(evt =>
            {
                mask.style.display = (MaskSource)evt.newValue == MaskSource.InitImageAlpha
                    ? DisplayStyle.None
                    : DisplayStyle.Flex;
                CodeHasChanged();
            });
            mask.style.display = (MaskSource)maskSource.value == MaskSource.InitImageAlpha
                ? DisplayStyle.None
                : DisplayStyle.Flex;

            engine.RegisterValueChangedCallback(_ => CodeHasChanged());

            CodeHasChanged();
        }
        
        public Action OnCodeHasChanged { get; set; }
        void CodeHasChanged()
        {
            OnCodeHasChanged?.Invoke();
        }

        public bool Valid()
        {
            imageRequired.style.visibility = Visibility.Hidden;
            maskRequired.style.visibility = Visibility.Hidden;
            if (!stabilityParameters.Valid())
            {
                return false;
            }

            if (image.image == null)
            {
                imageRequired.style.visibility = Visibility.Visible;
                return false;
            }

            var maskSourceValue = (MaskSource)maskSource.value;
            if (mask.image == null && maskSourceValue != MaskSource.InitImageAlpha)
            {
                maskRequired.style.visibility = Visibility.Visible;
                return false;
            }

            return true;
        }

        public void ApplyParameters(StabilityMaskedImageParameters stabilityMaskedImageParameters)
        {
            var maskSourceValue = (MaskSource)maskSource.value;
            stabilityMaskedImageParameters.EngineId = engine.value;
            stabilityMaskedImageParameters.InitImage = (Texture2D)image.image;
            stabilityMaskedImageParameters.MaskImage = maskSourceValue == MaskSource.InitImageAlpha ? null : (Texture2D)mask.image;
            stabilityMaskedImageParameters.MaskSource = maskSourceValue;
            stabilityParameters.ApplyParameters(stabilityMaskedImageParameters);
        }

        public string GetCode()
        {
            var maskSourceValue = (MaskSource)maskSource.value;
            return $"\t\tEngineId = \"{engine.value}\",\n" +
                   "\t\tInitImage = <Texture2D object>,\n" +
                   (maskSourceValue == MaskSource.InitImageAlpha ? "" : "\t\tMaskImage = <Texture2D object>,\n") +
                   $"\t\tMaskSource = MaskSource.{maskSourceValue},\n" +
                   stabilityParameters.GetCode();
        }
    }
}
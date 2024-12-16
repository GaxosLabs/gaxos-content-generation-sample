using System;
using System.Collections.Generic;
using ContentGeneration.Models;
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
            readonly UxmlBoolAttributeDescription _hidePrompt = new() { name = "HidePrompt" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var element = (MaskingParameters)ve;
                element.hidePrompt = _hidePrompt.GetValueFromBag(bag, cc);
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

        public bool hidePrompt
        {
            get => stabilityParameters.hidePrompt;
            set
            {
                stabilityParameters.hidePrompt = value;
                image.style.display =
                    maskSource.style.display =
                        mask.style.display = value ? DisplayStyle.None : DisplayStyle.Flex;
                if (!value)
                {
                    imageRequired.style.display =
                        maskRequired.style.display = DisplayStyle.None;
                }
            }
        }

        public MaskingParameters()
        {
            stabilityParameters.CodeHasChanged = CodeHasChanged;

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

            RegisterCallback<AttachToPanelEvent>(_ =>
                {
                    mask.style.display = (MaskSource)maskSource.value == MaskSource.InitImageAlpha
                        ? DisplayStyle.None
                        : DisplayStyle.Flex;
                });
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

        public Action codeHasChanged { get; set; }

        void CodeHasChanged()
        {
            codeHasChanged?.Invoke();
        }

        public bool Valid(bool updateUI)
        {
            if (updateUI)
            {
                imageRequired.style.visibility = Visibility.Hidden;
                maskRequired.style.visibility = Visibility.Hidden;
            }

            if (!stabilityParameters.Valid(updateUI))
            {
                return false;
            }

            if (image.image == null)
            {
                if (updateUI)
                {
                    imageRequired.style.visibility = Visibility.Visible;
                }

                return false;
            }

            var maskSourceValue = (MaskSource)maskSource.value;
            if (mask.image == null && maskSourceValue != MaskSource.InitImageAlpha)
            {
                if (updateUI)
                {
                    maskRequired.style.visibility = Visibility.Visible;
                }

                return false;
            }

            return true;
        }

        public void ApplyParameters(StabilityMaskedImageParameters stabilityMaskedImageParameters)
        {
            var maskSourceValue = (MaskSource)maskSource.value;
            stabilityMaskedImageParameters.EngineId = engine.value;
            stabilityMaskedImageParameters.InitImage = (Texture2D)image.image;
            stabilityMaskedImageParameters.MaskImage =
                maskSourceValue == MaskSource.InitImageAlpha ? null : (Texture2D)mask.image;
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

        public void Show(Favorite favorite)
        {
            var stabilityMaskedImageParameters = favorite.GeneratorParameters.ToObject<StabilityMaskedImageParameters>();
            stabilityParameters.Show(stabilityMaskedImageParameters);
            generationOptions.Show(favorite.GenerationOptions);
            
            engine.value = stabilityMaskedImageParameters.EngineId;
            maskSource.value = stabilityMaskedImageParameters.MaskSource;
            CodeHasChanged();
        }
    }
}
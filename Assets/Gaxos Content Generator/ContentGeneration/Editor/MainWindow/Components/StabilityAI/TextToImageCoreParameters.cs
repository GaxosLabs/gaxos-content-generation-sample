using System;
using System.Collections.Generic;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using ContentGeneration.Models.Stability;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.StabilityAI
{
    public class TextToImageCoreParameters : VisualElementComponent, IParameters<StabilityCoreTextToImageParameters>
    {
        public new class UxmlFactory : UxmlFactory<TextToImageCoreParameters, UxmlTraits>
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
                var element = (TextToImageCoreParameters)ve;
                element.hidePrompt = _hidePrompt.GetValueFromBag(bag, cc);
            }
        }

        PromptInput prompt => this.Q<PromptInput>("prompt");
        Button improvePromptButton => this.Q<Button>("improvePromptButton");
        VisualElement promptRequired => this.Q<VisualElement>("promptRequired");
        PromptInput negativePrompt => this.Q<PromptInput>("negativePrompt");
        EnumField aspectRatio => this.Q<EnumField>("aspectRatio");
        SliderInt seed => this.Q<SliderInt>("seed");
        Toggle sendStylePreset => this.Q<Toggle>("sendStylePreset");
        EnumField stylePreset => this.Q<EnumField>("stylePreset");
        EnumField outputFormat => this.Q<EnumField>("outputFormat");
        public GenerationOptionsElement generationOptions => this.Q<GenerationOptionsElement>("generationOptions");

        bool _hidePrompt;

        public bool hidePrompt
        {
            get => _hidePrompt;
            set
            {
                _hidePrompt = value;
                prompt.style.display =
                    improvePromptButton.style.display =
                        value ? DisplayStyle.None : DisplayStyle.Flex;
                if (value)
                {
                    promptRequired.style.display = DisplayStyle.None;
                }
            }
        }

        public TextToImageCoreParameters()
        {
            prompt.OnChanged += _ => CodeHasChanged();
            improvePromptButton.clicked += () =>
            {
                if (string.IsNullOrEmpty(prompt.value))
                    return;

                if (!improvePromptButton.enabledSelf)
                    return;

                improvePromptButton.SetEnabled(false);
                prompt.SetEnabled(false);
                ContentGenerationApi.Instance.ImprovePrompt(prompt.value, "midjourney").ContinueInMainThreadWith(
                    t =>
                    {
                        improvePromptButton.SetEnabled(true);
                        prompt.SetEnabled(true);

                        if (t.IsFaulted)
                        {
                            Debug.LogException(t.Exception!.InnerException!);
                            return;
                        }

                        prompt.value = t.Result;
                    });
            };

            negativePrompt.OnChanged += _ => CodeHasChanged();
            aspectRatio.RegisterValueChangedCallback(_ => CodeHasChanged());
            seed.RegisterValueChangedCallback(_ => CodeHasChanged());
            sendStylePreset.RegisterValueChangedCallback(evt =>
            {
                stylePreset.SetEnabled(evt.newValue);
                CodeHasChanged();
            });
            stylePreset.SetEnabled(sendStylePreset.value);
            stylePreset.RegisterValueChangedCallback(_ => CodeHasChanged());
            outputFormat.RegisterValueChangedCallback(_ => CodeHasChanged());

            CodeHasChanged();
        }

        public Action codeHasChanged { get; set; }

        void CodeHasChanged()
        {
            codeHasChanged?.Invoke();
        }

        public bool Valid(bool updateUI)
        {
            if (hidePrompt)
                return true;

            var thereArePrompts = !string.IsNullOrEmpty(prompt.value);

            if (updateUI)
            {
                promptRequired.style.visibility = thereArePrompts ? Visibility.Hidden : Visibility.Visible;
            }

            return !string.IsNullOrEmpty(prompt.value);
        }

        public void ApplyParameters(StabilityCoreTextToImageParameters stabilityParameters)
        {
            stabilityParameters.Prompt = prompt.value;
            stabilityParameters.AspectRatio = (AspectRatio)aspectRatio.value;
            stabilityParameters.NegativePrompt = negativePrompt.value;
            stabilityParameters.Seed = (ulong)seed.value;
            stabilityParameters.StylePreset =
                sendStylePreset.value ? (StylePreset)stylePreset.value : null;
            stabilityParameters.OutputFormat = (OutputFormat)outputFormat.value;
        }

        public string GetCode()
        {
            return
                $"\t\tPrompt = \"{prompt.value}\",\n" +
                $"\t\tAspectRatio = AspectRatio.{(AspectRatio)aspectRatio.value},\n" +
                (string.IsNullOrEmpty(negativePrompt.value) ? "" : $"\t\tNegativePrompt = \"{negativePrompt.value}\",\n") +
                $"\t\tSeed = {seed.value},\n" +
                (sendStylePreset.value ? $"\t\tStylePreset = StylePreset.{(StylePreset)stylePreset.value},\n" : "") +
                $"\t\tOutputFormat = OutputFormat.{(OutputFormat)outputFormat.value},\n";
        }

        public void Show(Favorite favorite)
        {
            var stabilityParameters = favorite.GeneratorParameters.ToObject<StabilityCoreTextToImageParameters>();
            generationOptions.Show(favorite.GenerationOptions);

            prompt.value = stabilityParameters.Prompt;
            aspectRatio.value = stabilityParameters.AspectRatio;
            negativePrompt.value = stabilityParameters.NegativePrompt;
            seed.value = (int)stabilityParameters.Seed;
            sendStylePreset.value = stabilityParameters.StylePreset.HasValue;
            if (stabilityParameters.StylePreset.HasValue)
            {
                stylePreset.value = stabilityParameters.StylePreset.Value;
            }
            outputFormat.value = stabilityParameters.OutputFormat;

            CodeHasChanged();
        }
    }
}
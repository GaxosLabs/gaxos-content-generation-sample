using System;
using System.Collections.Generic;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using ContentGeneration.Models.Stability;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.StabilityAI
{
    public class StableDiffusion3Parameters : VisualElementComponent, IParameters<StabilityStableDiffusion3Parameters>
    {
        public new class UxmlFactory : UxmlFactory<StableDiffusion3Parameters, UxmlTraits>
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
                var element = (StableDiffusion3Parameters)ve;
                element.hidePrompt = _hidePrompt.GetValueFromBag(bag, cc);
            }
        }

        PromptInput prompt => this.Q<PromptInput>("prompt");
        Button improvePromptButton => this.Q<Button>("improvePromptButton");
        VisualElement promptRequired => this.Q<VisualElement>("promptRequired");
        EnumField mode => this.Q<EnumField>("mode");
        ImageSelection image => this.Q<ImageSelection>("image");
        Label imageRequired => this.Q<Label>("imageRequired");
        Slider strength => this.Q<Slider>("strength");
        PromptInput negativePrompt => this.Q<PromptInput>("negativePrompt");
        EnumField model => this.Q<EnumField>("model");
        EnumField aspectRatio => this.Q<EnumField>("aspectRatio");
        SliderInt seed => this.Q<SliderInt>("seed");
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

        public StableDiffusion3Parameters()
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

            imageRequired.style.visibility = Visibility.Hidden;
            void ModeHasChanged(Mode value)
            {
                image.style.display = strength.style.display = imageRequired.style.display =
                    value == Mode.ImageToImage ? DisplayStyle.Flex : DisplayStyle.None;
            }

            mode.RegisterValueChangedCallback(v =>
            {
                ModeHasChanged((Mode)v.newValue);
                CodeHasChanged();
            });
            ModeHasChanged((Mode)mode.value);
            strength.RegisterValueChangedCallback(_ => CodeHasChanged());

            negativePrompt.OnChanged += _ => CodeHasChanged();
            model.RegisterValueChangedCallback(_ => CodeHasChanged());
            aspectRatio.RegisterValueChangedCallback(_ => CodeHasChanged());
            seed.RegisterValueChangedCallback(_ => CodeHasChanged());
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
            if (updateUI)
            {
                imageRequired.style.visibility = Visibility.Hidden;
            }

            var thereArePrompts = !string.IsNullOrEmpty(prompt.value);

            if (updateUI)
            {
                promptRequired.style.visibility = thereArePrompts ? Visibility.Hidden : Visibility.Visible;
            }

            if ((Mode)mode.value == Mode.TextToImage)
            {
                return !string.IsNullOrEmpty(prompt.value);
            }

            if (image.image == null)
            {
                if (updateUI)
                {
                    imageRequired.style.visibility = Visibility.Visible;
                }

                return false;
            }

            return true;
        }

        public void ApplyParameters(StabilityStableDiffusion3Parameters stabilityParameters)
        {
            stabilityParameters.Prompt = prompt.value;
            stabilityParameters.Mode = (Mode)mode.value;
            if (stabilityParameters.Mode == Mode.ImageToImage)
            {
                stabilityParameters.Image = (Texture2D)image.image;
                stabilityParameters.Strength = strength.value;
            }
            else
            {
                stabilityParameters.AspectRatio = (AspectRatio)aspectRatio.value;
            }

            stabilityParameters.Model = (Model)model.value;
            stabilityParameters.NegativePrompt = negativePrompt.value;
            stabilityParameters.Seed = (ulong)seed.value;
            stabilityParameters.OutputFormat = (OutputFormat)outputFormat.value;
        }

        public string GetCode()
        {
            var modeValue = (Mode)mode.value;
            return
                $"\t\tPrompt = \"{prompt.value}\",\n" +
                $"\t\tMode = Mode.{modeValue},\n" +
                (modeValue == Mode.ImageToImage ? $"\t\tImage = <Texture2D object>," : "") +
                (modeValue == Mode.ImageToImage ? $"\t\tStrength = {strength.value}f," : "") +
                $"\t\tModel = Model.{(Model)model.value},\n" +
                $"\t\tAspectRatio = AspectRatio.{(AspectRatio)aspectRatio.value},\n" +
                (string.IsNullOrEmpty(negativePrompt.value)
                    ? ""
                    : $"\t\tNegativePrompt = \"{negativePrompt.value}\",\n") +
                $"\t\tSeed = {seed.value},\n" +
                $"\t\tOutputFormat = OutputFormat.{(OutputFormat)outputFormat.value},\n";
        }

        public void Show(Favorite favorite)
        {
            var stabilityParameters = favorite.GeneratorParameters.ToObject<StabilityStableDiffusion3Parameters>();
            generationOptions.Show(favorite.GenerationOptions);

            prompt.value = stabilityParameters.Prompt;
            mode.value = stabilityParameters.Mode;
            if (stabilityParameters.Mode == Mode.ImageToImage)
            {
                strength.value = stabilityParameters.Strength;
            }
            else
            {
                aspectRatio.value = stabilityParameters.AspectRatio;
            }

            model.value = stabilityParameters.Model;
            negativePrompt.value = stabilityParameters.NegativePrompt;
            seed.value = (int)stabilityParameters.Seed;
            outputFormat.value = stabilityParameters.OutputFormat;
            
            CodeHasChanged();
        }
    }
}
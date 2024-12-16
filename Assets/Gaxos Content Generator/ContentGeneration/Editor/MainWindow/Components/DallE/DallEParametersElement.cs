using System;
using System.Collections.Generic;
using ContentGeneration.Helpers;
using ContentGeneration.Models.DallE;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.DallE
{
    public class DallEParametersElement : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<DallEParametersElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        PromptInput prompt => this.Q<PromptInput>("prompt");
        public EnumField model => this.Q<EnumField>("model");
        DropdownField resolution => this.Q<DropdownField>("resolution");
        SliderInt nSamples => this.Q<SliderInt>("nSamples");
        EnumField quality => this.Q<EnumField>("quality");
        EnumField generationStyle => this.Q<EnumField>("style");
        VisualElement promptRequired => this.Q<VisualElement>("promptRequired");
        Button improvePromptButton => this.Q<Button>("improvePromptButton");

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

        public DallEParametersElement()
        {
            prompt.OnChanged += _ => RefreshCode();
            improvePromptButton.clicked += () =>
            {
                if (string.IsNullOrEmpty(prompt.value))
                    return;

                if (!improvePromptButton.enabledSelf)
                    return;

                improvePromptButton.SetEnabled(false);
                prompt.SetEnabled(false);
                ContentGenerationApi.Instance.ImprovePrompt(prompt.value, "dall-e").ContinueInMainThreadWith(
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
            model.RegisterValueChangedCallback(ModeChanged);
            ModeChanged(ChangeEvent<Enum>.GetPooled(model.value, model.value));
            resolution.RegisterValueChangedCallback(_ => RefreshCode());
            nSamples.RegisterValueChangedCallback(_ => RefreshCode());
            quality.RegisterValueChangedCallback(_ => RefreshCode());
            generationStyle.RegisterValueChangedCallback(_ => RefreshCode());
        }

        public event Action OnCodeChanged;

        void RefreshCode()
        {
            OnCodeChanged?.Invoke();
        }

        void ModeChanged(ChangeEvent<Enum> evt)
        {
            resolution.choices.Clear();
            if ((Model)evt.newValue == Model.DallE2)
            {
                nSamples.highValue = 10;
                quality.style.display = DisplayStyle.None;
                resolution.choices.AddRange(new[]
                {
                    "256x256",
                    "512x512",
                    "1024x1024"
                });
                resolution.value = "1024x1024";
                generationStyle.style.display = DisplayStyle.None;
            }
            else
            {
                nSamples.value = 1;
                nSamples.highValue = 1;
                quality.style.display = DisplayStyle.Flex;
                resolution.choices.AddRange(new[]
                {
                    "1024x1024",
                    "1792x1024",
                    "1024x1792",
                });
                resolution.value = "1024x1024";
                generationStyle.style.display = DisplayStyle.Flex;
            }

            RefreshCode();
        }

        public bool Valid(bool updateUI)
        {
            if (hidePrompt)
                return true;

            var thereArePrompts = !string.IsNullOrEmpty(prompt.value);

            if(updateUI)
            {
                promptRequired.style.visibility = thereArePrompts ? Visibility.Hidden : Visibility.Visible;
            }

            return !string.IsNullOrEmpty(prompt.value);
        }

        public string GetCode()
        {
            var modelValue = (Model)model.value;
            var resolutionValue = resolution.value.Split('x', 2);
            var code =
                $"\t\tPrompt = {prompt.value},\n" +
                $"\t\tModel = {modelValue},\n" +
                $"\t\tN = {nSamples.value},\n" +
                (modelValue == Model.DallE2 ? "" : $"\t\tQuality = {quality.value},\n") +
                $"\t\tWidth = {resolutionValue[0]},\n" +
                $"\t\tHeight = {resolutionValue[1]},\n" +
                (modelValue == Model.DallE2 ? "" : $"\t\tStyle = {generationStyle.value},\n");
            return code;
        }

        public void ApplyParameters(DallEParameters dallEParameters)
        {
            var m = (Model)model.value;
            var r = resolution.value.Split('x', 2);

            dallEParameters.Prompt = prompt.value;
            dallEParameters.Model = m;
            dallEParameters.N = (uint)nSamples.value;
            dallEParameters.Quality = m == Model.DallE2 ? null : (Quality)quality.value;
            dallEParameters.Width = uint.Parse(r[0]);
            dallEParameters.Height = uint.Parse(r[1]);
            dallEParameters.Style = m == Model.DallE2 ? null : (Style)generationStyle.value;
        }

        public void Show(DallEParameters dallEParameters)
        {
            prompt.value = dallEParameters.Prompt;
            model.value = dallEParameters.Model;
            nSamples.value = (int)dallEParameters.N;
            quality.value = dallEParameters.Quality ?? Quality.Standard;
            resolution.value = $"{dallEParameters.Width}x{dallEParameters.Height}";
        }
    }
}
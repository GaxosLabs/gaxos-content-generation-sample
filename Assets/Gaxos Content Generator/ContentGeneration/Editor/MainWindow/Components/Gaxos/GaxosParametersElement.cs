using System;
using System.Collections.Generic;
using System.Linq;
using ContentGeneration.Helpers;
using ContentGeneration.Models.Gaxos;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Gaxos
{
    public class GaxosParametersElement : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<GaxosParametersElement, UxmlTraits>
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
        Button improvePromptButton => this.Q<Button>("improvePromptButton");
        VisualElement promptRequired => this.Q<VisualElement>("promptRequired");
        PromptInput negativePrompt => this.Q<PromptInput>("negativePrompt");

        TextField checkpoint => this.Q<TextField>("checkpoint");
        SliderInt nSamples => this.Q<SliderInt>("nSamples");
        Toggle sendSeed => this.Q<Toggle>("sendSeed");
        SliderInt seed => this.Q<SliderInt>("seed");
        SliderInt steps => this.Q<SliderInt>("steps");
        Slider cfg => this.Q<Slider>("cfg");
        TextField sampler => this.Q<TextField>("sampler");
        TextField scheduler => this.Q<TextField>("scheduler");
        Slider denoise => this.Q<Slider>("denoise");
        
        TextField loras => this.Q<TextField>("loras");
        
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

        public GaxosParametersElement()
        {
            prompt.OnChanged += _=> CodeHasChanged();
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
            
            negativePrompt.OnChanged += _  => CodeHasChanged();
            checkpoint.RegisterValueChangedCallback(_=> CodeHasChanged());
            nSamples.RegisterValueChangedCallback(_=> CodeHasChanged());
            sendSeed.RegisterValueChangedCallback(evt =>
            {
                seed.SetEnabled(evt.newValue);
                CodeHasChanged();
            });
            seed.SetEnabled(sendSeed.value);
            seed.RegisterValueChangedCallback(_=> CodeHasChanged());
            steps.RegisterValueChangedCallback(_=> CodeHasChanged());
            cfg.RegisterValueChangedCallback(_=> CodeHasChanged());
            sampler.RegisterValueChangedCallback(_=> CodeHasChanged());
            scheduler.RegisterValueChangedCallback(_=> CodeHasChanged());
            denoise.RegisterValueChangedCallback(_=> CodeHasChanged());
            loras.RegisterValueChangedCallback(_=> CodeHasChanged());
        }

        public Action OnCodeHasChanged;

        void CodeHasChanged()
        {
            OnCodeHasChanged?.Invoke();
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
            return 
                $"\t\tPrompt = \"{prompt.value}\",\n" +
                (!string.IsNullOrWhiteSpace(negativePrompt.value) ? $"\t\tNegativePrompt = \"{negativePrompt.value}\",\n" : null) +
                (!string.IsNullOrWhiteSpace(checkpoint.value) ? $"\t\tCheckpoint = \"{checkpoint.value}\",\n" : null) +
                $"\t\tNSamples = {nSamples.value},\n" +
                (sendSeed.value ? $"\t\tSeed = {seed.value},\n" : null) +
                $"\t\tSteps = {steps.value},\n" +
                $"\t\tCfg = {cfg.value},\n" +
                (!string.IsNullOrWhiteSpace(sampler.value) ? $"\t\tSamplerName = \"{sampler.value}\",\n" : null) +
                (!string.IsNullOrWhiteSpace(scheduler.value) ? $"\t\tScheduler = \"{scheduler.value}\",\n" : null) +
                $"\t\tDenoise = {denoise.value},\n" +
                (!string.IsNullOrWhiteSpace(loras.value) ? 
                    $"\t\tLoras = [{string.Join(", ", loras.value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => $"\"{i.Trim()}\""))}]\n" : null)
                ;
        }

        public void ApplyParameters(GaxosParameters gaxosParameters)
        {
            gaxosParameters.Prompt = prompt.value;
            gaxosParameters.NegativePrompt = string.IsNullOrWhiteSpace(negativePrompt.value) ? null : negativePrompt.value;
            gaxosParameters.Checkpoint = string.IsNullOrWhiteSpace(checkpoint.value) ? null : checkpoint.value;
            gaxosParameters.NSamples = (uint)nSamples.value;
            gaxosParameters.Seed = !sendSeed.value ? null : seed.value;
            gaxosParameters.Steps = (uint)steps.value;
            gaxosParameters.Cfg = cfg.value;
            gaxosParameters.SamplerName = string.IsNullOrWhiteSpace(sampler.value) ? null : sampler.value;
            gaxosParameters.Scheduler = string.IsNullOrWhiteSpace(scheduler.value) ? null : scheduler.value;
            gaxosParameters.Denoise = denoise.value;
            gaxosParameters.Loras = string.IsNullOrWhiteSpace(loras.value) ? null : 
                loras.value.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim()).ToArray();
        }

        public void Show(GaxosParameters gaxosParameters)
        {
            prompt.value = gaxosParameters.Prompt;
            negativePrompt.value = gaxosParameters.NegativePrompt;
            checkpoint.value = gaxosParameters.Checkpoint;
            nSamples.value = (int)gaxosParameters.NSamples;
            sendSeed.value = gaxosParameters.Seed != null;
            steps.value = (int)gaxosParameters.Steps;
            cfg.value = gaxosParameters.Cfg;
            sampler.value = gaxosParameters.SamplerName;
            scheduler.value = gaxosParameters.Scheduler;
            denoise.value = gaxosParameters.Denoise ?? 1;
            loras.value = gaxosParameters.Loras == null ? null :  string.Join(',', gaxosParameters.Loras);
        }
    }
}
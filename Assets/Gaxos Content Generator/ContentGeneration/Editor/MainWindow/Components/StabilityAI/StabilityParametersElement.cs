using System;
using System.Collections.Generic;
using System.Linq;
using ContentGeneration.Models.Stability;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.StabilityAI
{
    public class StabilityParametersElement : VisualElementComponent
    {
        public new class UxmlFactory : UxmlFactory<StabilityParametersElement, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        VisualElement promptsContainer => this.Q<VisualElement>("promptsContainer");
        Button addPrompt => this.Q<Button>("addPrompt");
        Label promptRequired => this.Q<Label>("promptRequiredLabel");
        SliderInt cfgScale => this.Q<SliderInt>("cfgScale");
        EnumField clipGuidancePreset => this.Q<EnumField>("clipGuidancePreset");
        Toggle sendSampler => this.Q<Toggle>("sendSampler");
        EnumField sampler => this.Q<EnumField>("sampler");
        SliderInt samples => this.Q<SliderInt>("samples");
        SliderInt seed => this.Q<SliderInt>("seed");
        SliderInt steps => this.Q<SliderInt>("steps");
        DropdownField stylePreset => this.Q<DropdownField>("stylePreset");

        public bool hidePrompt { get; set; }

        public Action CodeHasChanged;

        public StabilityParametersElement()
        {
            addPrompt.clicked += () =>
            {
                AddPrompt();
                CodeHasChanged?.Invoke();
            };
            promptRequired.style.visibility = Visibility.Hidden;

            cfgScale.RegisterValueChangedCallback(_ => CodeHasChanged?.Invoke());
            clipGuidancePreset.RegisterValueChangedCallback(_ => CodeHasChanged?.Invoke());

            sampler.RegisterValueChangedCallback(_ => CodeHasChanged?.Invoke());
            sendSampler.RegisterValueChangedCallback(evt =>
            {
                sampler.SetEnabled(evt.newValue);
                CodeHasChanged?.Invoke();
            });
            sampler.SetEnabled(sendSampler.value);

            samples.RegisterValueChangedCallback(_ => CodeHasChanged?.Invoke());
            seed.RegisterValueChangedCallback(_ => CodeHasChanged?.Invoke());
            steps.RegisterValueChangedCallback(_ => CodeHasChanged?.Invoke());
            stylePreset.choices.AddRange(new[]
            {
                "<None>",
                "3d-model",
                "analog-film",
                "anime",
                "cinematic",
                "comic-book",
                "digital-art",
                "enhance",
                "fantasy-art",
                "isometric",
                "line-art",
                "low-poly",
                "modeling-compound",
                "neon-punk",
                "origami",
                "photographic",
                "pixel-art",
                "tile-texture"
            });
            stylePreset.value = stylePreset.choices[0];
            stylePreset.RegisterValueChangedCallback(_ => CodeHasChanged?.Invoke());
        }

        void AddPrompt()
        {
            promptsContainer.Add(new TextPrompt(sender =>
                {
                    promptsContainer.Remove(sender);
                    CodeHasChanged?.Invoke();
                },
                () => CodeHasChanged?.Invoke()));
        }

        TextPrompt[] prompts
        {
            get
            {
                return promptsContainer.Children().Where(element => element is TextPrompt).Cast<TextPrompt>().ToArray();
            }
        }

        public bool Valid(bool updateUI)
        {
            if (hidePrompt)
            {
                return true;
            }
            
            var thereArePrompts = prompts.Length > 0 &&
                                  prompts.Any(p => !string.IsNullOrEmpty(p.prompt.Text));

            if(updateUI)
            {
                promptRequired.style.visibility = thereArePrompts ? Visibility.Hidden : Visibility.Visible;
            }

            return thereArePrompts;
        }

        public string GetCode()
        {
            var textPrompts = "";
            if (prompts.Length > 0)
            {
                textPrompts = string.Join(",\n", prompts.Select(p =>
                    "\t\t\tnew Prompt\n" +
                    "\t\t\t{\n" +
                    $"\t\t\t\tText = \"{p.prompt.Text}\",\n" +
                    $"\t\t\t\tWeight = {p.prompt.Weight}f,\n" +
                    "\t\t\t}\n"));
            }

            var code =
                "\t\tTextPrompts = new[]\n" +
                "\t\t{\n" +
                textPrompts +
                "\t\t},\n" +
                $"\t\tCfgScale = {(uint)cfgScale.value},\n" +
                $"\t\tClipGuidancePreset = ClipGuidancePreset.{(ClipGuidancePreset)clipGuidancePreset.value},\n" +
                $"\t\tSampler = {(sendSampler.value ? sampler.value.ToString() : "null")},\n" +
                $"\t\tSamples = {(uint)samples.value},\n" +
                $"\t\tSeed = {(uint)seed.value},\n" +
                $"\t\tSteps = {(uint)steps.value},\n" +
                ((!string.IsNullOrEmpty(stylePreset.value) && stylePreset.value != "<None>") ? $"\t\tStylePreset = \"{stylePreset.value}\",\n" : "");
            return code;
        }

        public void ApplyParameters(StabilityParameters stabilityParameters)
        {
            stabilityParameters.TextPrompts = prompts.Select(p => p.prompt).ToArray();
            stabilityParameters.CfgScale = (uint)cfgScale.value;
            stabilityParameters.ClipGuidancePreset = (ClipGuidancePreset)clipGuidancePreset.value;
            stabilityParameters.Sampler = sendSampler.value ? (Sampler)sampler.value : null;
            stabilityParameters.Samples = (uint)samples.value;
            stabilityParameters.Seed = (uint)seed.value;
            stabilityParameters.Steps = (uint)steps.value;
            if(!string.IsNullOrEmpty(stylePreset.value) && stylePreset.value != "<None>")
            {
                stabilityParameters.StylePreset = stylePreset.value;
            }
        }

        public void Show(StabilityParameters stabilityParameters)
        {
            while (promptsContainer.childCount < stabilityParameters.TextPrompts.Length)
            {
                AddPrompt();
            }

            while (promptsContainer.childCount > stabilityParameters.TextPrompts.Length)
            {
                promptsContainer.RemoveAt(0);
            }

            var prompts = this.prompts;
            for (var i = 0; i < stabilityParameters.TextPrompts.Length; i++)
            {
                prompts[i].prompt.Text = stabilityParameters.TextPrompts[i].Text;
                prompts[i].prompt.Weight = stabilityParameters.TextPrompts[i].Weight;
            }

            cfgScale.value = (int)stabilityParameters.CfgScale;
            clipGuidancePreset.value = stabilityParameters.ClipGuidancePreset;
            sendSampler.value = stabilityParameters.Sampler.HasValue;
            if (stabilityParameters.Sampler.HasValue)
            {
                sampler.value = stabilityParameters.Sampler.Value;
            }

            samples.value = (int)stabilityParameters.Samples;
            seed.value = (int)stabilityParameters.Seed;
            steps.value = (int)stabilityParameters.Steps;
            stylePreset.value = string.IsNullOrEmpty(stabilityParameters.StylePreset) ?
                "<None>" : stabilityParameters.StylePreset;
        }
    }
}
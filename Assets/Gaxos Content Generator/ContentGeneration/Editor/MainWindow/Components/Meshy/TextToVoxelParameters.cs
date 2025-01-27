using System;
using System.Collections.Generic;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using ContentGeneration.Models.Meshy;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Meshy
{
    public class TextToVoxelParameters : VisualElementComponent, IParameters<MeshyTextToVoxelParameters>
    {
        public new class UxmlFactory : UxmlFactory<TextToVoxelParameters, UxmlTraits>
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
                var element = (TextToVoxelParameters)ve;
                element.hidePrompt = _hidePrompt.GetValueFromBag(bag, cc);
            }
        }

        PromptInput prompt => this.Q<PromptInput>("prompt");
        VisualElement promptRequired => this.Q<VisualElement>("promptRequired");
        Button improvePromptButton => this.Q<Button>("improvePromptButton");

        EnumField voxelSizeShrinkFactor => this.Q<EnumField>("voxelSizeShrinkFactor");

        PromptInput negativePrompt => this.Q<PromptInput>("negativePrompt");
        
        Toggle sendSeed => this.Q<Toggle>("sendSeed");
        SliderInt seed => this.Q<SliderInt>("seed");


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
            }
        }

        public TextToVoxelParameters()
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
            voxelSizeShrinkFactor.RegisterValueChangedCallback(_ => CodeHasChanged());
            
            negativePrompt.OnChanged += _  => CodeHasChanged();
            sendSeed.RegisterValueChangedCallback(evt =>
            {
                seed.SetEnabled(evt.newValue);
                CodeHasChanged();
            });
            seed.SetEnabled(sendSeed.value);
            seed.RegisterValueChangedCallback(_=> CodeHasChanged());

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

            if(updateUI)
            {
                promptRequired.style.visibility = thereArePrompts ? Visibility.Hidden : Visibility.Visible;
            }

            return thereArePrompts;
        }

        public void ApplyParameters(MeshyTextToVoxelParameters parameters)
        {
            parameters.Prompt = prompt.value;
            parameters.VoxelSizeShrinkFactor = (VoxelSizeShrinkFactor)voxelSizeShrinkFactor.value;
            parameters.NegativePrompt = negativePrompt.value;
            parameters.Seed = sendSeed.value ? seed.value : null;
        }

        public string GetCode()
        {
            return $"\t\tPrompt = \"{prompt.value}\",\n" +
                   $"\t\tVoxelSizeShrinkFactor = VoxelSizeShrinkFactor.{voxelSizeShrinkFactor.value},\n" +
                   $"\t\tNegativePrompt = \"{negativePrompt.value}\",\n" +
                   (sendSeed.value ? $"\t\tHeight = {seed.value}\n" : "");
        }

        public void Show(Favorite favorite)
        {
            var parameters = favorite.GeneratorParameters.ToObject<MeshyTextToVoxelParameters>();
            prompt.value = parameters.Prompt;
            voxelSizeShrinkFactor.value = parameters.VoxelSizeShrinkFactor;
            negativePrompt.value = parameters.NegativePrompt;
            sendSeed.value = parameters.Seed.HasValue;
            seed.value = parameters.Seed ?? 0;
        }
    }
}
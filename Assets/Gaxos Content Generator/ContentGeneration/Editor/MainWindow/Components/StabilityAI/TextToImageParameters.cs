using System;
using System.Collections.Generic;
using System.Linq;
using ContentGeneration.Models;
using ContentGeneration.Models.Stability;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.StabilityAI
{
    public class TextToImageParameters : VisualElementComponent, IParameters<StabilityTextToImageParameters>
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

        DropdownField engine => this.Q<DropdownField>("engine");
        DropdownField width => this.Q<DropdownField>("width");
        DropdownField height => this.Q<DropdownField>("height");
        StabilityParametersElement stabilityParameters => this.Q<StabilityParametersElement>("stabilityParameters");
        public GenerationOptionsElement generationOptions => this.Q<GenerationOptionsElement>("generationOptions");

        public bool hidePrompt
        {
            get => stabilityParameters.hidePrompt;
            set => stabilityParameters.hidePrompt = value;
        }
        
        public TextToImageParameters()
        {
            stabilityParameters.CodeHasChanged += CodeHasChanged;
            generationOptions.OnCodeHasChanged = CodeHasChanged;

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

            string[] GetPossibleResolutions(string engineId)
            {
                uint xMin;
                uint xMax;
                uint yMin;
                uint yMax;
                switch (engineId)
                {
                    case "stable-diffusion-xl-1024-v0-9":
                    case "stable-diffusion-xl-1024-v1-0":
                        return new[]
                        {
                            "1024x1024",
                            "1152x896",
                            "1216x832",
                            "1344x768",
                            "1536x640",
                            "640x1536",
                            "768x1344",
                            "832x1216",
                            "896x1152"
                        };
                    case "stable-diffusion-v1-6":
                        xMin = 320;
                        xMax = 1536;
                        yMin = 320;
                        yMax = 1536;
                        break;
                    case "stable-diffusion-xl-beta-v2-2-2":
                        xMin = 128;
                        xMax = 512;
                        yMin = 128;
                        yMax = 512;
                        break;
                    case "esrgan-v1-x2plus":
                    case "stable-diffusion-512-v2-1":
                        xMin = 128;
                        xMax = 1536;
                        yMin = 128;
                        yMax = 1536;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(StabilityTextToImageParameters.EngineId), engineId);
                }

                var ret = new List<string>();
                for (var x = xMin; x <= xMax; x += 64)
                {
                    for (var y = yMin; y <= yMax; y += 64)
                    {
                        ret.Add($"{x}x{y}");
                    }
                }

                if (engineId == "stable-diffusion-xl-beta-v2-2-2")
                {
                    for (var i = 512 + 64; i <= 896; i += 64)
                    {
                        ret.Add($"512x{i}");
                        ret.Add($"{i}x512");
                    }
                }

                return ret.ToArray();
            }

            void RefreshResolutions(string newWidth = null, string newHeight = null)
            {
                var possibleResolutions = GetPossibleResolutions(engine.value);
                var widths = new HashSet<string>(possibleResolutions.
                    Select(i => i.Split('x')[0])).ToList();
                var heights = new HashSet<string>(possibleResolutions.
                        Select(i => i.Split('x')[1])).ToList();
                width.choices = widths;
                height.choices = heights;

                if (newWidth != null && !widths.Contains(newWidth))
                {
                    RefreshResolutions(null, newHeight);
                    return;
                }

                if (newHeight != null && !heights.Contains(newHeight))
                {
                    RefreshResolutions(newWidth, null);
                    return;
                }

                if (newWidth == null && newHeight == null)
                {
                    newWidth = widths[0]; 
                }

                if (newWidth != null)
                {
                    width.value = newWidth;
                    
                    var possibleHeights = new HashSet<string>(
                        possibleResolutions.
                            Where(i => i.StartsWith($"{width.value}x")).
                            Select(i => i.Split('x')[1])).ToList();
                    if (newHeight == null)
                    {
                        newHeight = height.value;
                    }
                    if (newHeight == null || !possibleHeights.Contains(newHeight))
                    {
                        newHeight = possibleHeights[0];
                    }
                    height.value = newHeight;
                }
                else
                {
                    height.value = newHeight;

                    var possibleWidths = (new HashSet<string>(
                        possibleResolutions.
                            Where(i => i.EndsWith($"x{height.value}")).
                            Select(i => i.Split('x')[0]))).ToList();
                    if (!possibleWidths.Contains(width.value))
                    {
                        width.value = possibleWidths[^1];
                    }
                }
            }

            engine.RegisterValueChangedCallback(_ =>
            {
                RefreshResolutions(width.value, height.value);
            });
            RefreshResolutions();

            engine.RegisterValueChangedCallback(_ => CodeHasChanged());
            width.RegisterValueChangedCallback(evt =>
            {
                RefreshResolutions(evt.newValue);
                CodeHasChanged();
            });
            height.RegisterValueChangedCallback(evt =>
            {
                RefreshResolutions(null, evt.newValue);
                CodeHasChanged();
            });

            CodeHasChanged();
        }

        public Action codeHasChanged { get; set; }
        void CodeHasChanged()
        {
            codeHasChanged?.Invoke();
        }

        public bool Valid(bool updateUI)
        {
            if (!stabilityParameters.Valid(updateUI))
            {
                return false;
            }

            return true;
        }

        public void ApplyParameters(StabilityTextToImageParameters stabilityParameters)
        {
            stabilityParameters.EngineId = engine.value;
            stabilityParameters.Width = uint.Parse(width.value);
            stabilityParameters.Height = uint.Parse(height.value);
            this.stabilityParameters.ApplyParameters(stabilityParameters);
        }

        public string GetCode()
        {
            return 
                $"\t\tEngineId = \"{engine.value}\",\n" +
                $"\t\tWidth = {uint.Parse(width.value)},\n" +
                $"\t\tHeight = {uint.Parse(height.value)},\n" +
                stabilityParameters.GetCode();
        }

        public void Show(Favorite favorite)
        {
            var stabilityParameters = favorite.GeneratorParameters.ToObject<StabilityTextToImageParameters>();
            this.stabilityParameters.Show(stabilityParameters);
            generationOptions.Show(favorite.GenerationOptions);
            
            engine.value = stabilityParameters.EngineId;
            width.value = stabilityParameters.Width.ToString();
            height.value = stabilityParameters.Height.ToString();

            CodeHasChanged();
        }
    }
}
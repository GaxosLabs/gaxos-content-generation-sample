using System;
using System.Collections.Generic;
using ContentGeneration.Helpers;
using ContentGeneration.Models;
using ContentGeneration.Models.Recraft;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.Recraft
{
    public class TextToImageParameters : VisualElementComponent, IParameters<RecraftTextToImageParameters>
    {
        public new class UxmlFactory : UxmlFactory<TextToImageParameters, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
        }

        public GenerationOptionsElement generationOptions => this.Q<GenerationOptionsElement>("generationOptions");
        public Action codeHasChanged { get; set; }

        PromptInput prompt => this.Q<PromptInput>("prompt");
        Button improvePromptButton => this.Q<Button>("improvePromptButton");
        VisualElement promptRequired => this.Q<VisualElement>("promptRequired");

        SliderInt n => this.Q<SliderInt>("n");
        DropdownField styleField => this.Q<DropdownField>("style");
        DropdownField substyle => this.Q<DropdownField>("substyle");
        EnumField model => this.Q<EnumField>("model");
        EnumField size => this.Q<EnumField>("size");

        Toggle sendMainColor => this.Q<Toggle>("sendMainColor");
        ColorField mainColor => this.Q<ColorField>("mainColor");
        Toggle sendBackgroundColor => this.Q<Toggle>("sendBackgroundColor");
        ColorField backgroundColor => this.Q<ColorField>("backgroundColor");

        public TextToImageParameters()
        {
            prompt.OnChanged += _ => codeHasChanged?.Invoke();
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
            n.RegisterValueChangedCallback(_ => codeHasChanged?.Invoke());
            styleField.RegisterValueChangedCallback(_ => RefreshSubStyles());
            model.RegisterValueChangedCallback(_ => RefreshStyles());
            size.RegisterValueChangedCallback(_ => codeHasChanged?.Invoke());

            sendMainColor.RegisterValueChangedCallback(_ => RefreshMainColor());
            mainColor.RegisterValueChangedCallback(_ => RefreshMainColor());
            sendBackgroundColor.RegisterValueChangedCallback(_ => RefreshBackgroundColor());
            backgroundColor.RegisterValueChangedCallback(_ => RefreshBackgroundColor());

            RefreshStyles();
            RefreshSubStyles();
            RefreshMainColor();
            RefreshBackgroundColor();
        }

        void RefreshMainColor()
        {
            mainColor.style.display = sendMainColor.value ? DisplayStyle.Flex : DisplayStyle.None;
            codeHasChanged?.Invoke();
        }

        void RefreshBackgroundColor()
        {
            backgroundColor.style.display = sendBackgroundColor.value ? DisplayStyle.Flex : DisplayStyle.None;
            codeHasChanged?.Invoke();
        }

        void RefreshStyles()
        {
            var currentStyle = styleField.value;
            if (string.IsNullOrEmpty(currentStyle))
            {
                currentStyle = Style.RealisticImage.ToString();
            }

            styleField.choices.Clear();
            var currentModel = (Model)model.value;
            switch (currentModel)
            {
                case Model.Recraftv3:
                    styleField.choices.Add(Style.RealisticImage.ToString());
                    styleField.choices.Add(Style.DigitalIllustration.ToString());
                    styleField.choices.Add(Style.VectorIllustration.ToString());
                    break;
                case Model.Recraft20b:
                    styleField.choices.Add(Style.RealisticImage.ToString());
                    styleField.choices.Add(Style.DigitalIllustration.ToString());
                    styleField.choices.Add(Style.VectorIllustration.ToString());
                    styleField.choices.Add(Style.Icon.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (styleField.choices.Contains(currentStyle))
            {
                styleField.value = currentStyle;
            }
            else
            {
                styleField.value = styleField.choices[0];
            }

            RefreshSubStyles();
        }

        void RefreshSubStyles()
        {
            var currentSubstyle = substyle.value;
            substyle.choices.Clear();
            var currentStyle = Enum.Parse<Style>(styleField.value);
            var currentModel = (Model)model.value;
            substyle.choices.Add("");
            switch (currentStyle)
            {
                case Style.RealisticImage:
                    if (currentModel == Model.Recraftv3)
                    {
                        substyle.choices.Add("b_and_w");
                        substyle.choices.Add("enterprise");
                        substyle.choices.Add("evening_light");
                        substyle.choices.Add("faded_nostalgia");
                        substyle.choices.Add("forest_life");
                        substyle.choices.Add("hard_flash");
                        substyle.choices.Add("hdr");
                        substyle.choices.Add("motion_blur");
                        substyle.choices.Add("mystic_naturalism");
                        substyle.choices.Add("natural_light");
                        substyle.choices.Add("natural_tones");
                        substyle.choices.Add("organic_calm");
                        substyle.choices.Add("real_life_glow");
                        substyle.choices.Add("retro_realism");
                        substyle.choices.Add("retro_snapshot");
                        substyle.choices.Add("studio_portrait");
                        substyle.choices.Add("urban_drama");
                        substyle.choices.Add("village_realism");
                        substyle.choices.Add("warm_folk");
                    }
                    else
                    {
                        substyle.choices.Add("b_and_w");
                        substyle.choices.Add("enterprise");
                        substyle.choices.Add("hard_flash");
                        substyle.choices.Add("hdr");
                        substyle.choices.Add("motion_blur");
                        substyle.choices.Add("natural_light");
                        substyle.choices.Add("studio_portrait");
                    }

                    break;
                case Style.DigitalIllustration:
                    if (currentModel == Model.Recraftv3)
                    {
                        substyle.choices.Add("2d_art_poster");
                        substyle.choices.Add("2d_art_poster_2");
                        substyle.choices.Add("engraving_color");
                        substyle.choices.Add("grain");
                        substyle.choices.Add("hand_drawn");
                        substyle.choices.Add("hand_drawn_outline");
                        substyle.choices.Add("handmade_3d");
                        substyle.choices.Add("infantile_sketch");
                        substyle.choices.Add("pixel_art");
                        substyle.choices.Add("antiquarian");
                        substyle.choices.Add("bold_fantasy");
                        substyle.choices.Add("child_book");
                        substyle.choices.Add("child_books");
                        substyle.choices.Add("cover");
                        substyle.choices.Add("crosshatch");
                        substyle.choices.Add("digital_engraving");
                        substyle.choices.Add("expressionism");
                        substyle.choices.Add("freehand_details");
                        substyle.choices.Add("grain_20");
                        substyle.choices.Add("graphic_intensity");
                        substyle.choices.Add("hard_comics");
                        substyle.choices.Add("long_shadow");
                        substyle.choices.Add("modern_folk");
                        substyle.choices.Add("multicolor");
                        substyle.choices.Add("neon_calm");
                        substyle.choices.Add("noir");
                        substyle.choices.Add("nostalgic_pastel");
                        substyle.choices.Add("outline_details");
                        substyle.choices.Add("pastel_gradient");
                        substyle.choices.Add("pastel_sketch");
                        substyle.choices.Add("pop_art");
                        substyle.choices.Add("pop_renaissance");
                        substyle.choices.Add("street_art");
                        substyle.choices.Add("tablet_sketch");
                        substyle.choices.Add("urban_glow");
                        substyle.choices.Add("urban_sketching");
                        substyle.choices.Add("vanilla_dreams");
                        substyle.choices.Add("young_adult_book");
                        substyle.choices.Add("young_adult_book_2");
                    }
                    else
                    {
                        substyle.choices.Add("2d_art_poster");
                        substyle.choices.Add("2d_art_poster_2");
                        substyle.choices.Add("3d");
                        substyle.choices.Add("80s");
                        substyle.choices.Add("engraving_color");
                        substyle.choices.Add("glow");
                        substyle.choices.Add("grain");
                        substyle.choices.Add("hand_drawn");
                        substyle.choices.Add("hand_drawn_outline");
                        substyle.choices.Add("handmade_3d");
                        substyle.choices.Add("infantile_sketch");
                        substyle.choices.Add("kawaii");
                        substyle.choices.Add("pixel_art");
                        substyle.choices.Add("psychedelic");
                        substyle.choices.Add("seamless");
                        substyle.choices.Add("voxel");
                        substyle.choices.Add("watercolor");
                    }

                    break;
                case Style.VectorIllustration:
                    if (currentModel == Model.Recraftv3)
                    {
                        substyle.choices.Add("bold_stroke");
                        substyle.choices.Add("chemistry");
                        substyle.choices.Add("colored_stencil");
                        substyle.choices.Add("contour_pop_art");
                        substyle.choices.Add("cosmics");
                        substyle.choices.Add("cutout");
                        substyle.choices.Add("depressive");
                        substyle.choices.Add("editorial");
                        substyle.choices.Add("emotional_flat");
                        substyle.choices.Add("engraving");
                        substyle.choices.Add("infographical");
                        substyle.choices.Add("line_art");
                        substyle.choices.Add("line_circuit");
                        substyle.choices.Add("linocut");
                        substyle.choices.Add("marker_outline");
                        substyle.choices.Add("mosaic");
                        substyle.choices.Add("naivector");
                        substyle.choices.Add("roundish_flat");
                        substyle.choices.Add("segmented_colors");
                        substyle.choices.Add("sharp_contrast");
                        substyle.choices.Add("thin");
                        substyle.choices.Add("vector_photo");
                        substyle.choices.Add("vivid_shapes");
                    }
                    else
                    {
                        substyle.choices.Add("cartoon");
                        substyle.choices.Add("doodle_line_art");
                        substyle.choices.Add("engraving");
                        substyle.choices.Add("flat_2");
                        substyle.choices.Add("kawaii");
                        substyle.choices.Add("line_art");
                        substyle.choices.Add("line_circuit");
                        substyle.choices.Add("linocut");
                        substyle.choices.Add("seamless");
                    }

                    break;
                case Style.Icon:
                    if (currentModel == Model.Recraftv3)
                    {
                    }
                    else
                    {
                        substyle.choices.Add("broken_line");
                        substyle.choices.Add("colored_outline");
                        substyle.choices.Add("colored_shapes");
                        substyle.choices.Add("colored_shapes_gradient");
                        substyle.choices.Add("doodle_fill");
                        substyle.choices.Add("doodle_offset_fill");
                        substyle.choices.Add("offset_fill");
                        substyle.choices.Add("outline");
                        substyle.choices.Add("outline_gradient");
                        substyle.choices.Add("uneven_fill");
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (substyle.choices.Contains(currentSubstyle))
            {
                substyle.value = currentSubstyle;
            }
            else
            {
                substyle.value = "";
            }

            codeHasChanged?.Invoke();
        }

        public bool Valid(bool updateUI)
        {
            var thereArePrompts = !string.IsNullOrEmpty(prompt.value);

            if(updateUI)
            {
                promptRequired.style.visibility = thereArePrompts ? Visibility.Hidden : Visibility.Visible;
            }

            return !string.IsNullOrEmpty(prompt.value);
        }

        public void ApplyParameters(RecraftTextToImageParameters parameters)
        {
            parameters.Prompt = prompt.value;
            parameters.N = n.value;
            parameters.Style = Enum.Parse<Style>(styleField.value);
            parameters.Substyle = string.IsNullOrEmpty(substyle.value) ? null : substyle.value;
            parameters.Model = (Model)model.value;
            parameters.Size = (Size)size.value;
            parameters.Controls = new Controls
            {
                Colors = sendMainColor.value ? new []{ mainColor.value } : Array.Empty<Color>(),
                BackgroundColor = sendBackgroundColor.value ? backgroundColor.value : null
            };
        }

        public string GetCode()
        {
            return 
                $"\t\tPrompt = \"{prompt.value}\",\n" +
                $"\t\tN = {n.value},\n" +
                $"\t\tStyle = Style.{styleField.value},\n" +
                $"\t\tSubstyle = {(string.IsNullOrEmpty(substyle.value) ? "null" : $"\"{substyle.value}\"")},\n" +
                $"\t\tModel = Model.{model.value},\n" +
                $"\t\tSize = Size.{size.value},\n" +
                $"\t\tControls = new Control,\n" +
                $"\t\t{{,\n" +
                $"\t\t\tColors = {(sendMainColor.value ? $"new []{{ new Color({mainColor.value.r}f, {mainColor.value.g}f, {mainColor.value.b}f) }}" : "Array.Empty<Color>()")},\n" +
                $"\t\t\tBackgroundColor = {(sendBackgroundColor.value ? $"new Color({backgroundColor.value.r}f, {backgroundColor.value.g}f, {backgroundColor.value.b}f)" : "null")},\n" +
                $"\t\t}}";
        }

        public void Show(Favorite favorite)
        {
            var parameters = favorite.GeneratorParameters.ToObject<RecraftTextToImageParameters>();
            
            prompt.value = parameters.Prompt;
            n.value = parameters.N;
            styleField.value = parameters.Style.ToString();
            substyle.value = parameters.Substyle;
            model.value = parameters.Model;
            size.value = parameters.Size;
            sendMainColor.value = parameters.Controls?.Colors?.Length > 0;
            if (sendMainColor.value)
            {
                mainColor.value = parameters.Controls!.Colors![0];
            }

            sendBackgroundColor.value = parameters.Controls?.BackgroundColor != null;
            if (sendBackgroundColor.value)
            {
                backgroundColor.value = parameters.Controls!.BackgroundColor!.Value;
            }
        }
    }
}
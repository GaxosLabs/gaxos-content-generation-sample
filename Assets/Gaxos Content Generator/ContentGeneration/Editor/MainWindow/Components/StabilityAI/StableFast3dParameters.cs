using System;
using System.Collections.Generic;
using ContentGeneration.Models;
using ContentGeneration.Models.Stability;
using UnityEngine;
using UnityEngine.UIElements;

namespace ContentGeneration.Editor.MainWindow.Components.StabilityAI
{
    public class StableFast3dParameters : VisualElementComponent, IParameters<StabilityStableFast3d>
    {
        public new class UxmlFactory : UxmlFactory<StableFast3dParameters, UxmlTraits>
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
                var element = (StableFast3dParameters)ve;
                element.hidePrompt = _hidePrompt.GetValueFromBag(bag, cc);
            }
        }

        ImageSelection image => this.Q<ImageSelection>("image");
        Label imageRequired => this.Q<Label>("imageRequired");
        EnumField textureResolution => this.Q<EnumField>("textureResolution");
        Slider foregroundRatio => this.Q<Slider>("foregroundRatio");
        EnumField remesh => this.Q<EnumField>("remesh");
        
        public GenerationOptionsElement generationOptions => null;

        bool _hidePrompt;
        public bool hidePrompt
        {
            get => _hidePrompt;
            set => _hidePrompt = value;
        }

        public StableFast3dParameters()
        {
            imageRequired.style.visibility = Visibility.Hidden;
            textureResolution.RegisterValueChangedCallback(_ => CodeHasChanged());
            foregroundRatio.RegisterValueChangedCallback(_ => CodeHasChanged());
            remesh.RegisterValueChangedCallback(_ => CodeHasChanged());
            image.OnChanged += CodeHasChanged;

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

        public void ApplyParameters(StabilityStableFast3d stabilityParameters)
        {
            stabilityParameters.Image =  (Texture2D)image.image;
            
            stabilityParameters.TextureResolution = (TextureResolution)textureResolution.value;
            stabilityParameters.ForegroundRatio = foregroundRatio.value;
            stabilityParameters.Remesh = (Remesh)remesh.value;
        }

        public string GetCode()
        {
            return
                $"\t\tImage = <Texture2D object>," +
                $"\t\tTextureResolution = TextureResolution.{(TextureResolution)textureResolution.value},\n" +
                $"\t\tForegroundRatio = {foregroundRatio.value}f," +
                $"\t\tRemesh = Remesh.{(Remesh)remesh.value},\n";
        }

        public void Show(Favorite favorite)
        {
            var stabilityParameters = favorite.GeneratorParameters.ToObject<StabilityStableFast3d>();
            
            textureResolution.value = stabilityParameters.TextureResolution;
            foregroundRatio.value = stabilityParameters.ForegroundRatio;
            remesh.value = stabilityParameters.Remesh;
            CodeHasChanged();
        }
    }
}